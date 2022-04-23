using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Types;
using NetTopologySuite.Geometries;
using ozgurtek.framework.common.Data;
using ozgurtek.framework.core.Data;

namespace ozgurtek.framework.driver.sqlserver
{
    public class GdMsSqlTable : GdAbstractDbTable
    {
        public GdMsSqlTable(GdMsSqlDataSource dataSource, string name, IGdFilter filter)
            : base(dataSource, name, filter)
        {
        }

        public override bool CanEditRow
        {
            get { return true; }
        }

        public override bool CanEditField
        {
            get { return false; }
        }

        public override bool CanSupportTransaction
        {
            get { return true; }
        }

        public override IGdTable Clone()
        {
            return new GdMsSqlTable(DataSource, Name, _filter);
        }

        protected override GdDataType? GetFieldType(DataRow row)
        {
            if (DbConvert.ToString(row["UdtAssemblyQualifiedName"]).Contains("SqlGeometry"))
                return GdDataType.Geometry;

            return null;
        }

        protected override IDbDataParameter CreateParameter(IGdParamater parameter)
        {
            SqlParameter result = new SqlParameter();
            result.ParameterName = parameter.Name;

            if (DbConvert.IsDbNull(parameter.Value))
            {
                result.Value = DBNull.Value;
            }
            else if (parameter.Value is Geometry geometry)
            {
                result.SqlDbType = SqlDbType.Udt;
                result.UdtTypeName = "Geometry";
                SqlBytes bytes = new SqlBytes(geometry.ToBinary());
                SqlGeometry sqlGeometry = SqlGeometry.STGeomFromWKB(bytes, geometry.SRID);
                result.Value = sqlGeometry;
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
                        result.SqlDbType = SqlDbType.Binary;
                        break;
                    case GdDataType.Geometry:
                        result.SqlDbType = SqlDbType.Udt;
                        result.UdtTypeName = "Geometry";
                        break;
                    case GdDataType.Boolean:
                        result.SqlDbType = SqlDbType.Bit;
                        break;
                    case GdDataType.Date:
                        result.SqlDbType = SqlDbType.Date;
                        break;
                    case GdDataType.Integer:
                        result.SqlDbType = SqlDbType.Int;
                        break;
                    case GdDataType.Real:
                        result.SqlDbType = SqlDbType.Real;
                        break;
                    case GdDataType.String:
                        result.SqlDbType = SqlDbType.NVarChar;
                        break;
                }
            }

            return result;
        }

        protected override GdRowBuffer CreateRowBuffer()
        {
            return new GdMsSqlRowBuffer();
        }

        protected override long ExecuteInsert(IGdRowBuffer row, IDbCommand command)
        {
            List<string> valuesPart = new List<string>();
            List<string> keys = new List<string>();
            foreach (IGdParamater param in row.Paramaters)
            {
                keys.Add(param.Name);
                valuesPart.Add($"@{param.Name}");
                command.Parameters.Add(CreateParameter(param));
            }
            
            string keysStr = string.Join(",", keys);
            string valuespartStr = string.Join(",", valuesPart);

            string outputStr = "";
            if (!string.IsNullOrEmpty(KeyField))
                outputStr = $"OUTPUT Inserted.{KeyField}";

            string query = $"INSERT INTO {Name} ({keysStr}) {outputStr} VALUES({valuespartStr})";
            command.CommandText = string.Format(query, Name, keysStr, valuespartStr);
            object value = command.ExecuteScalar();

            return Convert.ToInt64(value);
        }

        protected override long ExecuteUpdate(IGdRowBuffer row, IDbCommand command)
        {
            List<string> parameters = new List<string>();
            List<string> keys = new List<string>();
            foreach (IGdParamater param in row.Paramaters)
            {
                keys.Add(param.Name);
                parameters.Add($"{param.Name}=@{param.Name}");
                command.Parameters.Add(CreateParameter(param));
            }

            command.CommandText = $"UPDATE {Name} SET {string.Join(",", parameters)} WHERE {KeyField} = {row.GetAsInteger(KeyField)}";
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
            throw new System.NotSupportedException();
        }

        protected override int ExecuteDeleteField(IGdField field, IDbCommand command)
        {
            throw new System.NotSupportedException();
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
                limit = " FETCH NEXT " + Limit + " ROWS ONLY ";

            string offset = "";
            if (_offset >= 0)
                offset = " OFFSET " + Offset + " ROWS ";

            filter.Text = $"SELECT {columnFilter} FROM ({filter.Text}) A {orderBy} {offset} {limit}";
        }

        protected override void AppendGeometryFilter(IGdFilter filter)
        {
            if (GeometryFilter == null)
                return;

            if (string.IsNullOrEmpty(GeometryField))
                throw new Exception("GeometryFilter exists but GeometryField empty or null");

            string stMask = ToSTMask(GeometryFilter.SpatialRelation);
            string spatialClause = $"{GeometryField}.{stMask}(@{GeometryField})=1";
            filter.Text = $"SELECT * FROM ({filter.Text}) A WHERE {spatialClause}";
            filter.Add(GeometryField, GeometryFilter.Geometry);
        }

        private string ToSTMask(GdSpatialRelation mask)
        {
            string result;
            switch (mask)
            {
                case GdSpatialRelation.Disjoint:
                    result = "STDisjoint";
                    break;

                case GdSpatialRelation.Intersects:
                    result = "STIntersects";
                    break;

                case GdSpatialRelation.Touches:
                    result = "STTouches";
                    break;

                case GdSpatialRelation.Crosses:
                    result = "STCrosses";
                    break;

                case GdSpatialRelation.Within:
                    result = "STWithin";
                    break;

                case GdSpatialRelation.Contains:
                    result = "STContains";
                    break;

                case GdSpatialRelation.Overlaps:
                    result = "STOverlaps";
                    break;

                case GdSpatialRelation.Equals:
                    result = "STEquals";
                    break;

                default:
                    result = null;
                    break;
            }

            return result;
        }

        public GdMsSqlDataSource DataSource
        {
            get { return (GdMsSqlDataSource) _dataSource; }
        }
    }
}