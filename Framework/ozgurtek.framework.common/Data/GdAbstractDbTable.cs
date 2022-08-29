using ozgurtek.framework.core.Data;
using System;
using System.Collections.Generic;
using System.Data;

namespace ozgurtek.framework.common.Data
{
    public abstract class GdAbstractDbTable : GdAbstractTable, IGdDbTable
    {
        protected readonly GdAbstractDbDataSource _dataSource;

        protected readonly IGdFilter _filter; //base filter
        protected IGdFilter _sqlFilter; //sql filter

        protected GdSchema _schema;
        protected IGdGeometryFilter _geometryFilter;
        protected string _columnFilter;
        protected string _orderBy;
        protected long _limit = -1;
        protected long _offset = -1;

        //for transaction...
        protected IDbConnection _connection;
        protected IDbTransaction _transaction;

        protected GdAbstractDbTable(GdAbstractDbDataSource dataSource, string name, IGdFilter filter)
        {
            _dataSource = dataSource;
            _filter = filter;
            Name = name;
        }

        public override IGdSchema Schema
        {
            get
            {
                if (_schema != null)
                    return _schema;

                _schema = new GdSchema();

                using (IDbConnection connection = _dataSource.GetConnection())
                {
                    using (IDbCommand command = connection.CreateCommand())
                    {
                        IGdFilter filter = CreateFilter();
                        command.CommandText = $"SELECT * FROM ({filter.Text}) A WHERE 0 = 1";
                        foreach (IGdParamater parameter in filter.Parameters)
                            command.Parameters.Add(CreateParameter(parameter));

                        using (IDataReader reader = command.ExecuteReader())
                        {
                            DataTable schemaTable = reader.GetSchemaTable();
                            if (schemaTable == null)
                                throw new Exception("schemaTable not found");

                            foreach (DataRow row in schemaTable.Rows)
                            {
                                GdDataType? dataType = null;
                                string dataTypeStr = DbConvert.ToString(row["DataType"]);

                                Type type = Type.GetType(dataTypeStr);
                                if (type != null)
                                {
                                    GdDataTypeConverter converter = new GdDataTypeConverter();
                                    dataType = converter.ToGdDataType(type);
                                }

                                if (dataType == null)
                                    dataType = GetFieldType(row);

                                if (dataType == null)
                                    continue;
                                
                                GdField field = new GdField();
                                field.FieldName = DbConvert.ToString(row["ColumnName"]);
                                //field.NotNull = !DbConvert.ToBoolean(row["AllowDBNull"]);//todo:postgres'ten düzgün gelmiyor, bu yüzden kapattım.
                                field.FieldType = dataType.Value;
                                _schema.Add(field);
                            }
                        }

                        return _schema;
                    }
                }
            }
        }

        public virtual IGdFilter SqlFilter
        {
            get { return _sqlFilter; }
            set { _sqlFilter = value; }
        }

        public virtual string ColumnFilter
        {
            get { return _columnFilter; }
            set
            {
                _schema = null;
                _columnFilter = value;
            }
        }

        public virtual string OrderBy
        {
            get { return _orderBy; }
            set { _orderBy = value; }
        }

        public virtual long Limit
        {
            get { return _limit; }
            set { _limit = value; }
        }

        public virtual long Offset
        {
            get { return _offset; }
            set { _offset = value; }
        }

        public override long RowCount
        {
            get
            {
                using (IDbConnection connection = _dataSource.GetConnection())
                {
                    using (IDbCommand command = connection.CreateCommand())
                    {
                        IGdFilter filter = CreateFilter();
                        command.CommandText = $"SELECT COUNT(*) FROM ({filter.Text}) A";
                        foreach (IGdParamater parameter in filter.Parameters)
                            command.Parameters.Add(CreateParameter(parameter));

                        return DbConvert.ToInt32(command.ExecuteScalar());
                    }
                }
            }
        }

        public override IEnumerable<IGdRow> Rows
        {
            get
            {
                using (IDbConnection connection = _dataSource.GetConnection())
                {
                    using (IDbCommand command = connection.CreateCommand())
                    {
                        IGdFilter filter = CreateFilter();
                        command.CommandText = filter.Text;
                        foreach (IGdParamater parameter in filter.Parameters)
                            command.Parameters.Add(CreateParameter(parameter));

                        using (IDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                GdRowBuffer buffer = CreateRowBuffer();
                                buffer.Table = this;

                                int fieldCount = reader.FieldCount;
                                for (int i = 0; i < fieldCount; i++)
                                {
                                    string name = reader.GetName(i);
                                    object value = reader[i];
                                    buffer.Put(name, value);
                                }

                                yield return buffer;
                            }
                        }
                    }
                }
            }
        }

        public override IGdRow FindRow(long rowId)
        {
            if (string.IsNullOrEmpty(KeyField))
                throw new Exception($"Please specify key field of {Name} table");

            GdDataType dataType = Schema.GetFieldByName(KeyField).FieldType;
            if (!(dataType == GdDataType.Integer || dataType == GdDataType.Real))
                throw new Exception($"{KeyField} column must be integer or real type");

            using (IDbConnection connection = _dataSource.GetConnection())
            {
                using (IDbCommand command = connection.CreateCommand())
                {
                    IGdFilter filter = CreateFilter();
                    command.CommandText = $"SELECT * FROM ({filter.Text}) A WHERE {KeyField} = {rowId}";
                    foreach (IGdParamater parameter in filter.Parameters)
                        command.Parameters.Add(CreateParameter(parameter));

                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            GdRowBuffer buffer = CreateRowBuffer();
                            buffer.Table = this;

                            int fieldCount = reader.FieldCount;
                            for (int i = 0; i < fieldCount; i++)
                            {
                                string name = reader.GetName(i);
                                object value = reader[i];
                                buffer.Put(name, value);
                            }

                            return buffer;
                        }
                    }
                }
            }

            return null;
        }

        public override IEnumerable<object> GetDistinctValues(string fieldName)
        {
            using (IDbConnection connection = _dataSource.GetConnection())
            {
                using (IDbCommand command = connection.CreateCommand())
                {
                    IGdFilter filter = CreateFilter();
                    command.CommandText = $"SELECT DISTINCT({fieldName}) FROM ({filter.Text}) A";
                    foreach (IGdParamater parameter in filter.Parameters)
                        command.Parameters.Add(CreateParameter(parameter));

                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            yield return reader[0];
                        }
                    }
                }
            }
        }

        public override long Insert(IGdRowBuffer row)
        {
            if (!CanEditRow)
                throw new Exception("Can not modify table");

            if (_sqlFilter != null || _geometryFilter != null)
                throw new Exception("Can not modify filtered table");

            if (!string.IsNullOrWhiteSpace(KeyField))
            {
                IGdField field = Schema.GetFieldByName(KeyField);
                if (field == null)
                    throw new Exception($"Can not find keyfield {KeyField} of shema");

                if (!(field.FieldType == GdDataType.Integer || field.FieldType == GdDataType.Real))
                    throw new Exception($"{KeyField} column must be integer or real type");
            }

            if (_connection != null) //use transaction
            {
                using (IDbCommand command = _connection.CreateCommand())
                {
                    command.Transaction = _transaction;
                    long result = ExecuteInsert(row, command);
                    OnRowChanged("Insert", row);
                    return result;
                }
            }

            //immediately update...
            using (IDbConnection connection = _dataSource.GetConnection())
            {
                using (IDbCommand command = connection.CreateCommand())
                {
                    long result = ExecuteInsert(row, command);
                    OnRowChanged("Insert", row);
                    return result;
                }
            }
        }

        public override long Update(IGdRowBuffer row)
        {
            if (!CanEditRow)
                throw new Exception("Can not modify table");

            if (_sqlFilter != null || _geometryFilter != null)
                throw new Exception("Can not modify filtered table");

            if (string.IsNullOrEmpty(KeyField))
                throw new Exception("Can not find keyfield defination of table");

            if (!row.ContainsKey(KeyField))
                throw new Exception($"Can not find keyfield {KeyField} of row buffer");

            IGdField field = Schema.GetFieldByName(KeyField);
            if (field == null)
                throw new Exception($"Can not find keyfield {KeyField} of Shema");

            if (!(field.FieldType == GdDataType.Integer || field.FieldType == GdDataType.Real))
                throw new Exception($"{KeyField} column must be integer or real type");

            if (_connection != null) //use transaction
            {
                using (IDbCommand command = _connection.CreateCommand())
                {
                    command.Transaction = _transaction;
                    long result = ExecuteUpdate(row, command);
                    OnRowChanged("update", row);
                    return result;
                }
            }

            //immediately update...
            using (IDbConnection connection = _dataSource.GetConnection())
            {
                using (IDbCommand command = connection.CreateCommand())
                {
                    long result = ExecuteUpdate(row, command);
                    OnRowChanged("update", row);
                    return result;
                }
            }
        }

        public override long Delete(long id)
        {
            if (!CanEditRow)
                throw new Exception("Can not modify table");

            if (_sqlFilter != null || _geometryFilter != null)
                throw new Exception("Can not modify filtered table");

            if (string.IsNullOrEmpty(KeyField))
                throw new Exception("Can not find keyfield defination of table");

            IGdField field = Schema.GetFieldByName(KeyField);
            if (field == null)
                throw new Exception($"Can not find keyfield {KeyField} of Shema");

            if (!(field.FieldType == GdDataType.Integer || field.FieldType == GdDataType.Real))
                throw new Exception($"{KeyField} column must be integer or real type");

            //backup row
            IGdRow row = null;
            if (HasRowChangeSubscriptions)
                row = FindRow((int) id);

            if (_connection != null) //use transaction
            {
                using (IDbCommand command = _connection.CreateCommand())
                {
                    command.Transaction = _transaction;
                    long result = ExecuteDelete(id, command);
                    if (row != null)
                        OnRowChanged("delete", (IGdRowBuffer) row);
                    return result;
                }
            }

            //immediately delete...
            using (IDbConnection connection = _dataSource.GetConnection())
            {
                using (IDbCommand command = connection.CreateCommand())
                {
                    long result = ExecuteDelete(id, command);
                    if (row != null)
                        OnRowChanged("delete", (IGdRowBuffer) row);
                    return result;
                }
            }
        }

        public override void Truncate()
        {
            if (_connection != null) //use transaction
            {
                using (IDbCommand command = _connection.CreateCommand())
                {
                    command.Transaction = _transaction;
                    ExecuteTruncate(command);
                    return;
                }
            }

            //immediately delete...
            using (IDbConnection connection = _dataSource.GetConnection())
            {
                using (IDbCommand command = connection.CreateCommand())
                {
                    ExecuteTruncate(command);
                }
            }
        }

        public virtual void BeginTransaction()
        {
            if (!CanSupportTransaction)
                throw new Exception("Can not support transaction");

            if (!CanEditRow)
                throw new Exception("Can not modify table");

            if (_connection != null)
                throw new Exception("Transaction Already Exists");

            _connection = _dataSource.GetConnection();
            _transaction = _connection.BeginTransaction(IsolationLevel.Serializable);
        }

        public virtual void CommitTransaction()
        {
            if (!CanSupportTransaction)
                throw new Exception("Can not support transaction");

            if (!CanEditRow)
                throw new Exception("Can not modify table");

            if (_connection == null)
                throw new Exception("Transaction not exists, use beginTransaction");

            _transaction.Commit();
            _transaction.Dispose();
            _connection.Dispose();
        }

        public virtual void RollBackTransaction()
        {
            if (!CanSupportTransaction)
                throw new Exception("Can not support transaction");

            if (!CanEditRow)
                throw new Exception("Can not modify table");

            if (_connection == null)
                throw new Exception("Transaction not exists, use beginTransaction");

            _transaction.Rollback();
            _transaction.Dispose();
            _connection.Dispose();
        }

        public override void CreateField(IGdField field)
        {
            if (!CanEditField)
                throw new Exception("Can not modify table");

            if (_sqlFilter != null || _geometryFilter != null)
                throw new Exception("Can not modify filtered table");

            if (_connection != null) //use transaction
            {
                using (IDbCommand command = _connection.CreateCommand())
                {
                    command.Transaction = _transaction;
                    ExecuteCreateField(field, command);
                    return;
                }
            }

            //immediately update...
            using (IDbConnection connection = _dataSource.GetConnection())
            {
                using (IDbCommand command = connection.CreateCommand())
                {
                    ExecuteCreateField(field, command);
                }
            }
        }

        public override void DeleteField(IGdField field)
        {
            if (!CanEditField)
                throw new Exception("Can not modify table");

            if (_sqlFilter != null || _geometryFilter != null)
                throw new Exception("Can not modify filtered table");

            if (_connection != null) //use transaction
            {
                using (IDbCommand command = _connection.CreateCommand())
                {
                    command.Transaction = _transaction;
                    ExecuteDeleteField(field, command);
                    return;
                }
            }

            //immediately update...
            using (IDbConnection connection = _dataSource.GetConnection())
            {
                using (IDbCommand command = connection.CreateCommand())
                {
                    ExecuteDeleteField(field, command);
                }
            }
        }

        public virtual bool CanSupportTransaction
        {
            get { return false; }
        }

        protected virtual IGdFilter CreateFilter()
        {
            GdSqlFilter result = new GdSqlFilter();

            AppendBaseQuery(result);
            AppendSqlFilter(result);
            AppendEnvelopeFilter(result);
            AppendGeometryFilter(result);
            EndAppend(result);

            return result;
        }

        protected virtual void AppendBaseQuery(IGdFilter filter)
        {
            filter.Text = $"SELECT * FROM ({_filter.Text}) A {string.Empty}";
            filter.AddRange(_filter.Parameters);
        }

        protected virtual void AppendSqlFilter(IGdFilter filter)
        {
            if (SqlFilter == null)
                return;

            filter.Text = $"SELECT * FROM ({_filter.Text}) A WHERE {SqlFilter.Text}";
            filter.AddRange(SqlFilter.Parameters);
        }

        protected virtual void AppendGeometryFilter(IGdFilter filter)
        {
        }

        protected virtual void AppendEnvelopeFilter(IGdFilter filter)
        {
        }

        protected virtual void EndAppend(IGdFilter filter)
        {
        }

        protected abstract GdDataType? GetFieldType(DataRow row);
        protected abstract long ExecuteInsert(IGdRowBuffer row, IDbCommand command);
        protected abstract long ExecuteUpdate(IGdRowBuffer row, IDbCommand command);
        protected abstract long ExecuteDelete(long id, IDbCommand command);
        protected abstract void ExecuteTruncate(IDbCommand command);
        protected abstract int ExecuteCreateField(IGdField field, IDbCommand command);
        protected abstract int ExecuteDeleteField(IGdField field, IDbCommand command);
        protected abstract IDbDataParameter CreateParameter(IGdParamater parameter);
        protected abstract GdRowBuffer CreateRowBuffer();
    }
}