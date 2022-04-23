using ozgurtek.framework.common.Data;
using ozgurtek.framework.core.Data;
using System;
using System.Collections.Generic;
using System.Data;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using Oracle.DataAccess.Client;
using ozgurtek.framework.driver.oracle.NetTopologySuit.IO.Oracle;
using ozgurtek.framework.driver.oracle.NetTopologySuit.IO.Oracle.Sdo;


namespace ozgurtek.framework.driver.oracle
{
    public class GdOracleTable : GdAbstractDbTable
    {
        internal GdOracleTable(GdOracleDataSource dataSource, string name, IGdFilter filter)
            : base(dataSource, name, filter)
        {
        }

        protected override void EndAppend(IGdFilter filter)
        {
            string columnFilter = "*";
            if (!string.IsNullOrEmpty(ColumnFilter))
                columnFilter = ColumnFilter;

            string orderBy = "";
            if (!string.IsNullOrEmpty(OrderBy))
                orderBy = " ORDER BY " + OrderBy;

            string offset = "";
            string prefix = "";
            if (_offset >= 0)
            {
                offset = " WHERE rnum >= " + Offset;
                prefix = ", rownum AS rnum";
            }

            string limit = "";
            if (_limit >= 0)
            {
                limit = " WHERE rownum < " + (Limit + Offset);
            }

            filter.Text = $"SELECT {columnFilter} FROM (SELECT A.* {prefix} FROM ({filter.Text} {orderBy})A {limit}) B {offset}";
        }

        protected override void AppendGeometryFilter(IGdFilter filter)
        {
            if (GeometryFilter == null)
                return;

            if (string.IsNullOrEmpty(GeometryField))
                throw new Exception("GeometryFilter exists but GeometryField empty or null");

            string equalsOperator = "=";
            if (GeometryFilter.SpatialRelation == GdSpatialRelation.Disjoint)
                equalsOperator = "!=";

            string sdoMask = ToSdoMask(GeometryFilter.SpatialRelation);
            string spatialClause = $"SDO_RELATE({GeometryField}, :otherGeo, 'MASK={sdoMask} querytype=WINDOW') {equalsOperator} 'TRUE' ";
            filter.Text = $"SELECT * FROM ({filter.Text}) A WHERE {spatialClause}";
            filter.Add("otherGeo", GeometryFilter.Geometry);
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
            return new GdOracleTable(DataSource, Name, _filter);
        }

        protected override long ExecuteInsert(IGdRowBuffer row, IDbCommand command)
        {
            List<string> valuesPart = new List<string>();
            List<string> keys = new List<string>();
            
            foreach (IGdParamater param in row.Paramaters)
            {
                keys.Add(param.Name);
                valuesPart.Add($":{param.Name}");
                command.Parameters.Add(CreateParameter(param));
            }

            string query = "INSERT INTO {0} ({1}) VALUES({2})";
            if (!string.IsNullOrEmpty(KeyField))
                query += " RETURNING " + KeyField + " INTO :RESULT ";

            command.CommandText = string.Format(query, Name, string.Join(",", keys), string.Join(",", valuesPart));
            OracleParameter outParameter = ((OracleCommand)command).Parameters.Add("RESULT", OracleDbType.Int32);
            outParameter.Direction = ParameterDirection.Output;
            command.ExecuteScalar();

            return long.Parse(outParameter.Value.ToString());
        }

        protected override long ExecuteUpdate(IGdRowBuffer row, IDbCommand command)
        {
            List<string> parameters = new List<string>();
            List<string> keys = new List<string>();

            foreach (IGdParamater param in row.Paramaters)
            {
                keys.Add(param.Name);
                parameters.Add(string.Format("{0}={1}", param.Name, $":{param.Name}"));
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
            throw new NotSupportedException();
        }

        protected override int ExecuteDeleteField(IGdField field, IDbCommand command)
        {
            throw new NotSupportedException();
        }

        protected override IDbDataParameter CreateParameter(IGdParamater parameter)
        {
            OracleParameter result;
            if (DbConvert.IsDbNull(parameter.Value))
            {
                result = new OracleParameter(parameter.Name, DBNull.Value);
            }
            else if (parameter.Value is bool blValue)
            {
                result = new OracleParameter(parameter.Name, DbConvert.ToInt32(blValue));
            }
            else if (parameter.Value is Geometry)
            {
                Geometry geometry = (Geometry)parameter.Value;

                WKBWriter wkbWriter = new WKBWriter();
                wkbWriter.HandleOrdinates = Ordinates.XY;
                byte[] bytes = wkbWriter.Write(geometry);

                WKBReader reader = new WKBReader();
                reader.HandleOrdinates = Ordinates.XY;
                Geometry tempGeom = reader.Read(bytes);
                tempGeom.SRID = geometry.SRID;

                OracleGeometryWriter writer = new OracleGeometryWriter();
                writer.SRID = geometry.SRID;
                SdoGeometry sdoGeometry = writer.Write(tempGeom);

                result = new OracleParameter();
                result.ParameterName = parameter.Name;
                result.Value = sdoGeometry;
                result.OracleDbType = OracleDbType.Object;
                result.UdtTypeName = "MDSYS.SDO_GEOMETRY";
            }
            else
                result = new OracleParameter(parameter.Name, parameter.Value);

            //force set datatype
            if (parameter.DataType.HasValue)
            {
                switch (parameter.DataType)
                {
                    case GdDataType.Blob:
                        result.OracleDbType = OracleDbType.Blob;
                        break;
                    case GdDataType.Geometry:
                        result.OracleDbType = OracleDbType.Object;
                        result.UdtTypeName = "MDSYS.SDO_GEOMETRY";
                        break;
                    case GdDataType.Boolean:
                        result.OracleDbType = OracleDbType.Int32;
                        break;
                    case GdDataType.Date:
                        result.OracleDbType = OracleDbType.Date;
                        break;
                    case GdDataType.Integer:
                        result.OracleDbType = OracleDbType.Int32;
                        break;
                    case GdDataType.Real:
                        result.OracleDbType = OracleDbType.Double;
                        break;
                    case GdDataType.String:
                        result.OracleDbType = OracleDbType.NVarchar2;
                        break;
                }
            }

            return result;
        }

        protected override GdRowBuffer CreateRowBuffer()
        {
            return new GdOracleRowBuffer();
        }

        //protected override GdDataType ConvertToDataType(DataColumn column)
        //{
        //    if (column.DataType.Name == "SdoGeometry")
        //        return GdDataType.Geometry;

        //    GdDataTypeConverter converter = new GdDataTypeConverter();
        //    return converter.ToGdDataType(column.DataType);
        //}

        protected override GdDataType? GetFieldType(DataRow row)
        {
            if (DbConvert.ToString(row["DataType"]).Contains("SdoGeometry"))
                return GdDataType.Geometry;

            return null;
        }

        private static string ToSdoMask(GdSpatialRelation value)
        {
            string result = string.Empty;
            foreach (GdSpatialRelation relation in Enum.GetValues(typeof(GdSpatialRelation)))
            {
                if (value != relation)
                    continue;

                string word;
                switch (relation)
                {
                    case GdSpatialRelation.Disjoint:
                        word = "ANYINTERACT"; //Ayrık Olduğu - OK
                        break;

                    case GdSpatialRelation.Intersects:
                        word = "ANYINTERACT"; //Kesiştiği - OK
                        break;

                    case GdSpatialRelation.Touches:
                        word = "TOUCH"; //Değdiği - OK
                        break;

                    case GdSpatialRelation.Crosses:
                        word = "OVERLAPBDYDISJOINT"; //Kestiği - OK
                        break;

                    case GdSpatialRelation.Within:
                        word = "INSIDE"; //İçinde Olduğu - OK - WITHIN_DISTANCE
                        break;

                    case GdSpatialRelation.Contains:
                        word = "CONTAINS"; //Kapsadığı - OK - INSIDE
                        break;

                    case GdSpatialRelation.Equals:
                        word = "EQUALS"; //Eşit Olduğu - OK
                        break;

                    case GdSpatialRelation.Overlaps:
                        word = "OVERLAPBDYDISJOINT+OVERLAPBDYINTERSECT"; //Çakıştığı - OK
                        break;

                    default:
                        word = null;
                        break;
                }

                if (!string.IsNullOrEmpty(word))
                {
                    if (result.Length > 0)
                        result += "+";
                    result += word;
                }
            }

            return result;
        }

        public GdOracleDataSource DataSource
        {
            get { return (GdOracleDataSource)_dataSource; }
        }
    }
}