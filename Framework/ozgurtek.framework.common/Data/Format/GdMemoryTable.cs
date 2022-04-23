using ozgurtek.framework.core.Data;
using System;
using System.Collections.Generic;
using System.Data;
using NetTopologySuite.Geometries;
using ozgurtek.framework.common.Util;

namespace ozgurtek.framework.common.Data.Format
{
    public class GdMemoryTable : GdAbstractTable, IGdDbTable, IDisposable
    {
        private readonly DataTable _dataTable = new DataTable();
        private IGdFilter _sqlFilter;
        private string _keyField;

        public string ColumnFilter { get; set; }
        public string OrderBy { get; set; }
        public long Limit { get; set; } = -1;
        public long Offset { get; set; } = -1;

        public override IEnumerable<IGdRow> Rows
        {
            get
            {
                foreach (DataRow dataRow in ApplyFilter())
                    yield return ToGdRow(dataRow);
            }
        }

        public override string Name
        {
            get { return _dataTable.TableName; }
            set { _dataTable.TableName = value; }
        }

        public override string KeyField
        {
            get { return _keyField; }
            set { _keyField = value; }
        }

        public override IGdSchema Schema
        {
            get
            {
                GdSchema schema = new GdSchema();
                GdDataTypeConverter converter = new GdDataTypeConverter();
                foreach (DataColumn dataColumn in _dataTable.Columns)
                    schema.Add(new GdField(dataColumn.ColumnName, converter.ToGdDataType(dataColumn.DataType)));

                return schema;
            }
        }

        public override long Insert(IGdRowBuffer row)
        {
            if (row.Size() == 0)
                throw new Exception("buffer field size 0");

            DataRow dataRow = _dataTable.NewRow();
            IEnumerable<IGdParamater> paramaters = row.Paramaters;
            foreach (IGdParamater paramater in paramaters)
            {
                object val = DBNull.Value;
                if (paramater.Value != null)
                    val = paramater.Value;

                dataRow[paramater.Name] = val;
            }

            //todo enis burayı impelente et şimdlik herzaman bir dön.
            _dataTable.Rows.Add(dataRow);

            OnRowChanged("Insert", row);
            return 1;
        }

        public override long Update(IGdRowBuffer row)
        {
            if (string.IsNullOrEmpty(KeyField))
                throw new Exception("KeyField Missing...");

            int dataRow = row.GetAsInteger(KeyField);

            DataRow finded = null;
            foreach (DataRow tableRow in _dataTable.Rows)
            {
                if (Convert.ToInt32(tableRow[KeyField]) != dataRow)
                    continue;

                finded = tableRow;
                break;
            }

            if (finded == null)
                throw new Exception("Can't find row");

            IEnumerable<IGdParamater> paramaters = row.Paramaters;

            foreach (IGdParamater paramater in paramaters)
            {
                object val = DBNull.Value;
                if (paramater.Value != null)
                    val = paramater.Value;

                finded[paramater.Name] = val;
            }

            //todo enis burayı impelente et şimdlik herzaman bir dön.
            OnRowChanged("update", row);
            return 1;
        }

        public override long Delete(long id)
        {
            if (string.IsNullOrEmpty(KeyField))
                throw new Exception("KeyField Missing...");

            foreach (DataRow tableRow in _dataTable.Rows)
            {
                if (Convert.ToInt32(tableRow[KeyField]) != id)
                    continue;

                _dataTable.Rows.Remove(tableRow);
                OnRowChanged("delete", null);

                return 1;
            }

            return -1;
        }

        public override void Truncate()
        {
            _dataTable.Rows.Clear();
        }

        public override void CreateField(IGdField field)
        {
            GdDataTypeConverter converter = new GdDataTypeConverter();
            Type dotNetType = converter.ToDotNetType(field.FieldType);
            DataColumn column = _dataTable.Columns.Add(field.FieldName, dotNetType);

            //not null
            column.AllowDBNull = !field.NotNull;

            //default value
            if (!string.IsNullOrWhiteSpace(field.DefaultVal))
                column.DefaultValue = field.DefaultVal;

            //primary key
            if (field.PrimaryKey)
            {
                List<DataColumn> primaryKeys = new List<DataColumn>(_dataTable.PrimaryKey);
                DataColumn item = _dataTable.Columns[field.FieldName];
                item.AutoIncrement = true;
                item.AutoIncrementSeed = 1;
                primaryKeys.Add(item);
                _dataTable.PrimaryKey = primaryKeys.ToArray();
            }
        }

        public override void DeleteField(IGdField field)
        {
            foreach (DataColumn column in _dataTable.Columns)
            {
                if (string.Equals(field.FieldName, column.ColumnName, StringComparison.OrdinalIgnoreCase))
                {
                    _dataTable.Columns.Remove(column);
                    break;
                }
            }
        }

        public override bool CanEditRow
        {
            get { return true; }
        }

        public bool CanSupportTransaction
        {
            get { return false; }
        }

        public override bool CanEditField
        {
            get { return true; }
        }

        public override IGdTable Clone()
        {
            return new GdMemoryTable();
        }

        public virtual void Dispose()
        {
            _dataTable.Dispose();
        }

        public void BeginTransaction()
        {
            throw new NotSupportedException();
        }

        public void CommitTransaction()
        {
            throw new NotSupportedException();
        }

        public void RollBackTransaction()
        {
            throw new NotSupportedException();
        }

        public IGdFilter SqlFilter
        {
            get => _sqlFilter;
            set => _sqlFilter = value;
        }

        public DataTable ToDataTable()
        {
            return _dataTable.Copy();
        }

        public static GdMemoryTable LoadFromTable(IGdTable table)
        {
            GdMemoryTable result = new GdMemoryTable();
            
            //todo: aşağıdakilerden emin değilim.
            //result.Name = table.Name;
            //result.KeyField = table.KeyField;
            //result.GeometryField = table.GeometryField;
            //result.Description = table.Description;

            IGdSchema schema = table.Schema;
            foreach (IGdField otherSchemaField in schema.Fields)
                result.CreateField(otherSchemaField);

            foreach (IGdRow row in table.Rows)
            {
                GdRowBuffer buffer = new GdRowBuffer(row);
                result.Insert(buffer);
            }

            return result;
        }

        public static GdMemoryTable LoadFromJson(string json, bool useSchema = true)
        {
            return new GdJsonTableDeserializer().Deserialize(json, useSchema);
        }

        protected virtual IGdRow ToGdRow(DataRow dataRow)
        {
            GdRowBuffer buffer = new GdRowBuffer();
            buffer.Table = this;
            foreach (DataColumn dataColumn in dataRow.Table.Columns)
                buffer.Put(dataColumn.ColumnName, dataRow[dataColumn]);
            return buffer;
        }

        protected virtual IEnumerable<DataRow> ApplyFilter()
        {
            DataView dataView = new DataView(_dataTable);

            //apply sql filter if exists
            string sqlFilter = CreateSql();
            if (!string.IsNullOrEmpty(sqlFilter))
                dataView.RowFilter = sqlFilter;

            //apply order by
            if (!string.IsNullOrWhiteSpace(OrderBy))
                dataView.Sort = OrderBy;

            //apply geometry filter if exists
            long count = 0;
            long take = 0;
            DataTable dataTable = dataView.ToTable();
            foreach (DataRow dataRow in dataView.ToTable().Rows)
            {
                if (GeometryFilter != null && !string.IsNullOrEmpty(GeometryField))
                {
                    Geometry geometry = dataRow[GeometryField] as Geometry;
                    if (DbConvert.IsDbNull(geometry))
                        continue;

                    if (!GdGeometryUtil.Relate(geometry, GeometryFilter.Geometry, GeometryFilter.SpatialRelation))
                        continue;
                }

                count++;
                if (Offset > 0 && count <= Offset)
                    continue;

                take++;
                if (Limit > 0 && take > Limit)
                    break;

                yield return dataRow;
            }

            dataTable.Dispose();
        }

        private string CreateSql()
        {
            if (_sqlFilter == null)
                return null;

            string result = _sqlFilter.Text;
            foreach (IGdParamater parameter in _sqlFilter.Parameters)
            {
                string val = null;
                if (!DbConvert.IsDbNull(parameter.Value))
                    val = parameter.Value.ToString();

                if (parameter.Value is string || parameter.Value is DateTime)
                    val = $"'{parameter.Value}'";

                result = result.Replace("@" + parameter.Name, val);
            }

            return result;
        }
    }
}