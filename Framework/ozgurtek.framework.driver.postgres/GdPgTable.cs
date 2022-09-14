using NetTopologySuite.Geometries;
using Npgsql;
using NpgsqlTypes;
using ozgurtek.framework.common.Data;
using ozgurtek.framework.core.Data;
using System;
using System.Collections.Generic;
using System.Data;

namespace ozgurtek.framework.driver.postgres
{
    public class GdPgTable : GdAbstractDbTable
    {
        internal GdPgTable(GdPgDataSource dataSource, string name, IGdFilter filter)
            : base(dataSource, name, filter)
        {
        }

        protected override void AppendGeometryFilter(IGdFilter filter)
        {
            if (GeometryFilter == null)
                return;

            if (string.IsNullOrEmpty(GeometryField))
                throw new Exception("GeometryFilter exists but GeometryField empty or null");

            string stMask = ToSTMask(GeometryFilter.SpatialRelation);
            string geometry = $"public.st_geomfromwkb(@otherGeo, {GeometryFilter.Geometry.SRID})";
            string spatialClause = $"{stMask}({GeometryField},{geometry})";
            filter.Text = $"SELECT * FROM ({filter.Text}) A WHERE {spatialClause}";
            filter.Add("otherGeo", GeometryFilter.Geometry);
        }

        public override bool CanEditRow
        {
            get { return true; }
        }

        public override bool CanEditField
        {
            get { return true; }
        }

        public override bool CanSupportTransaction
        {
            get { return true; }
        }

        public override IGdTable Clone()
        {
            return new GdPgTable(DataSource, Name, _filter);
        }

        public override Envelope Envelope
        {
            get
            {
                using (IDbConnection connection = _dataSource.GetConnection())
                {
                    using (IDbCommand command = connection.CreateCommand())
                    {
                        IGdFilter filter = CreateFilter();
                        command.CommandText = $"SELECT ST_Envelope(ST_Collect({GeometryField})) FROM ({filter.Text}) A";
                        foreach (IGdParamater parameter in filter.Parameters)
                            command.Parameters.Add(CreateParameter(parameter));

                        using (IDataReader reader = command.ExecuteReader())
                        {
                            bool read = reader.Read();
                            object value = reader.GetValue(0);
                            if (value is Geometry geometry)
                                return geometry.EnvelopeInternal;
                        }
                    }
                }

                return null;
            }
        }

        protected override IDbDataParameter CreateParameter(IGdParamater parameter)
        {
            NpgsqlParameter result = new NpgsqlParameter();
            result.ParameterName = parameter.Name;

            if (DbConvert.IsDbNull(parameter.Value))
            {
                result.Value = DBNull.Value;
            }
            else if (parameter.Value is Geometry)
            {
                result.NpgsqlDbType = NpgsqlDbType.Geometry;
                result.Value = (Geometry) (parameter.Value);
            }
            else
            {
                result.Value = parameter.Value;
            }

            //force set datatype
            if (parameter.DataType.HasValue)
            {
                switch (parameter.DataType)
                {
                    case GdDataType.Blob:
                        result.NpgsqlDbType = NpgsqlDbType.Bytea;
                        break;
                    case GdDataType.Geometry:
                        result.NpgsqlDbType = NpgsqlDbType.Geometry;
                        break;
                    case GdDataType.Boolean:
                        result.NpgsqlDbType = NpgsqlDbType.Boolean;
                        break;
                    case GdDataType.Date:
                        result.NpgsqlDbType = NpgsqlDbType.Date;
                        break;
                    case GdDataType.Integer:
                        result.NpgsqlDbType = NpgsqlDbType.Integer;
                        break;
                    case GdDataType.Real:
                        result.NpgsqlDbType = NpgsqlDbType.Double;
                        break;
                    case GdDataType.String:
                        result.NpgsqlDbType = NpgsqlDbType.Varchar;
                        break;
                }
            }

            return result;
        }

        protected override void EndAppend(IGdFilter filter)
        {
            string columnFilter = "*";
            if (!string.IsNullOrEmpty(ColumnFilter))
                columnFilter = ColumnFilter;

            string orderBy = "";
            if (!string.IsNullOrEmpty(OrderBy))
                orderBy = " ORDER BY " + OrderBy;

            string limit = "";
            if (_limit >= 0)
                limit = " LIMIT " + Limit;

            string offset = "";
            if (_offset >= 0)
                offset = " OFFSET " + Offset;

            filter.Text = $"SELECT {columnFilter} FROM ({filter.Text}) A {orderBy} {limit} {offset}";
        }

        protected override GdRowBuffer CreateRowBuffer()
        {
            return new GdPgRowBuffer();
        }

        protected override GdDataType? GetFieldType(DataRow row)
        {
            if (DbConvert.ToString(row["DataType"]).Contains("NetTopologySuite.Geometries.Geometry"))
                return GdDataType.Geometry;

            return null;
        }

        protected override long ExecuteInsert(IGdRowBuffer row, IDbCommand command)
        {
            List<string> parameters = new List<string>();
            List<string> keys = new List<string>();

            foreach (IGdParamater paramater in row.Paramaters)
            {
                keys.Add(paramater.Name);
                parameters.Add(CreateParameterSqlPart(paramater));
                command.Parameters.Add(CreateParameter(paramater));
            }

            string query = "INSERT INTO {0} ({1}) VALUES({2})";
            if (!string.IsNullOrEmpty(KeyField))
                query += " RETURNING " + KeyField;

            command.CommandText = string.Format(query, Name, string.Join(",", keys), string.Join(",", parameters));
            return DbConvert.ToInt32(command.ExecuteScalar());
        }

        protected override long ExecuteUpdate(IGdRowBuffer row, IDbCommand command)
        {
            List<string> parameters = new List<string>();
            foreach (IGdParamater paramater in row.Paramaters)
            {
                parameters.Add($"{paramater.Name}={CreateParameterSqlPart(paramater)}");
                command.Parameters.Add(CreateParameter(paramater));
            }

            command.CommandText =
                $"UPDATE {Name} SET {string.Join(",", parameters)} WHERE {KeyField} = {row.GetAsInteger(KeyField)}";
            return DbConvert.ToInt32(command.ExecuteScalar());
        }

        protected override long ExecuteDelete(long id, IDbCommand command)
        {
            command.CommandText = $"DELETE FROM {Name} WHERE {KeyField} = {id}";
            return DbConvert.ToInt32(command.ExecuteScalar());
        }

        protected override void ExecuteTruncate(IDbCommand command)
        {
            command.CommandText = $"truncate table {Name}";
            command.ExecuteNonQuery();
        }

        protected override int ExecuteCreateField(IGdField field, IDbCommand command)
        {
            //create field
            string dataTypeString = string.Empty;
            if (field.FieldType == GdDataType.Geometry)
            {
                string geomTypeString = "geometry";
                if (field.GeometryType.HasValue)
                    geomTypeString = "multi" + field.GeometryType.Value;

                if (field.Srid > 0)
                    dataTypeString = $"geometry({geomTypeString},{field.Srid})";
                else
                    dataTypeString = $"geometry({geomTypeString})";
            }
            else if (field.FieldType == GdDataType.Blob)
                dataTypeString = "bytea";
            else
                dataTypeString = GetDataTypeString(field.FieldType);

            string notNullStr = string.Empty;
            if (field.NotNull)
                notNullStr = "NOT NULL";

            string defaultStr = string.Empty;
            if (!string.IsNullOrWhiteSpace(field.DefaultVal))
                defaultStr = "DEFAULT " + field.DefaultVal;

            string sql = $"alter table {Name} add column {field.FieldName} {dataTypeString} {notNullStr} {defaultStr}";
            command.CommandText = sql;
            command.ExecuteNonQuery();

            //add primary key
            if (field.PrimaryKey)
            {
                sql = $"Alter table {Name} Add PRIMARY KEY ({field.FieldName})";
                command.CommandText = sql;
                command.ExecuteNonQuery();
            }

            return 1;
        }

        protected override int ExecuteDeleteField(IGdField field, IDbCommand command)
        {
            string sql = $"Alter Table {Name} Drop Column {field.FieldName}";
            command.CommandText = sql;
            return command.ExecuteNonQuery();
        }

        protected string CreateParameterSqlPart(IGdParamater parameter)
        {
            if (parameter.Value is Geometry)
                return $"public.st_geomfromwkb(@{parameter.Name}, {((Geometry) parameter.Value).SRID})";
            return $"@{parameter.Name}";
        }

        private string ToSTMask(GdSpatialRelation mask)
        {
            string result;
            switch (mask)
            {
                case GdSpatialRelation.Disjoint:
                    result = "public.ST_Disjoint";
                    break;

                case GdSpatialRelation.Intersects:
                    result = "public.ST_Intersects";
                    break;

                case GdSpatialRelation.Touches:
                    result = "public.ST_Touches";
                    break;

                case GdSpatialRelation.Crosses:
                    result = "public.ST_Crosses";
                    break;

                case GdSpatialRelation.Within:
                    result = "public.ST_Within";
                    break;

                case GdSpatialRelation.Contains:
                    result = "public.ST_Contains";
                    break;

                case GdSpatialRelation.Overlaps:
                    result = "public.ST_Overlaps";
                    break;

                case GdSpatialRelation.Equals:
                    result = "public.ST_Equals";
                    break;

                default:
                    result = null;
                    break;
            }

            return result;
        }

        public GdPgDataSource DataSource
        {
            get { return (GdPgDataSource) _dataSource; }
        }

        private string GetDataTypeString(GdDataType dataType)
        {
            switch (dataType)
            {
                case GdDataType.Blob:
                    return "blob";
                case GdDataType.Geometry:
                    return "geometry";
                case GdDataType.Integer:
                    return "integer";
                case GdDataType.Boolean:
                    return "boolean";
                case GdDataType.Real:
                    return "double precision";
                case GdDataType.Date:
                    return "timestamp(0)";
                case GdDataType.String:
                    return "text";
                default:
                    throw new Exception("unsupported data type " + dataType);
            }
        }
    }
}