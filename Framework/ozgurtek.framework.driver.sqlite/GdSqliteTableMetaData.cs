using NetTopologySuite.Geometries;
using ozgurtek.framework.common.Data;
using ozgurtek.framework.common.Util;
using ozgurtek.framework.core.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ozgurtek.framework.driver.sqlite
{
    internal class GdSqliteTableMetaData
    {
        private GdSchema _schema;
        private readonly GdSqlLiteTable _table;
        private readonly GdSqlLiteConnection _connection;
        private GdGeometryType? _geometryType;
        private string _geometryField;
        private string _keyField;
        private int? _srid;

        public GdSqliteTableMetaData(GdSqlLiteTable table, GdSqlLiteConnection connection)
        {
            _table = table;
            _connection = connection;
        }

        public GdGeometryType? GeometryType
        {
            get
            {
                if (string.IsNullOrWhiteSpace(GeometryFieldName))
                    throw new Exception("GeometryFieldName Missing...");

                if (_geometryType.HasValue)
                    return _geometryType.Value;

                GdSqliteField field = (GdSqliteField)Schema.GetFieldByName(GeometryFieldName);
                _geometryType = field.GeometryType;
                return _geometryType.Value;
            }
            set
            {
                _geometryType = value;
            }
        }

        public string GeometryFieldName
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(_geometryField))
                    return _geometryField;

                foreach (IGdField field in Schema)
                {
                    GdSqliteField sqliteField = (GdSqliteField)field;
                    if (sqliteField.FieldType == GdDataType.Geometry)
                        return _geometryField = field.FieldName;
                }
                return null;
            }
            set
            {
                _geometryField = value;
            }
        }

        public virtual string KeyField
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(_keyField))
                    return _keyField;

                foreach (IGdField field in Schema)
                {
                    if (field.PrimaryKey)
                        return _keyField = field.FieldName;
                }
                return null;
            }
            set { _keyField = value; }
        }

        public virtual int SRID
        {
            get
            {
                if (string.IsNullOrWhiteSpace(GeometryFieldName))
                    throw new Exception("GeometryFieldName Missing...");

                if (_srid.HasValue)
                    return _srid.Value;

                GdSqliteField field = (GdSqliteField)Schema.GetFieldByName(GeometryFieldName);
                _srid = field.Srid;
                return _srid.Value;
            }
            set { _srid = value; }
        }

        public GdSchema Schema
        {
            get
            {
                if (_schema != null)
                    return _schema;

                _schema = new GdSchema();
                FillFromSystemTable(_schema);
                FillFromGeometryTable(_schema);

                return _schema;
            }
        }

        private void FillFromSystemTable(GdSchema schema)
        {
            string sql = "pragma table_info({0})";
            string query = string.Format(sql, _table.Name);
            IEnumerable<IGdRow> rows = _connection.ExecuteReader(query, _table);
            foreach (IGdRow row in rows)
            {
                GdSqliteField field = new GdSqliteField();
                field.Table = _table;
                field.FieldName = row.GetAsString("name");
                field.NotNull = row.GetAsInteger("notnull") > 0;
                field.PrimaryKey = row.GetAsInteger("pk") > 0;
                field.DefaultVal = row.GetAsString("dflt_value");
                field.FieldType = GetDataTypeFromString(row.GetAsString("type"));
                schema.Add(field);
            }
        }

        private void FillFromGeometryTable(GdSchema schema)
        {
            foreach (IGdField schemaField in schema)
            {
                GdSqliteField field = (GdSqliteField)schemaField;

                string sql = "select geometry_type gt, geometry_format gf, srid " +
                             "from " +
                             "geometry_columns " +
                             "where " +
                             "f_table_name = '{0}' and " +
                             "f_geometry_column ='{1}'";

                string query = string.Format(sql, _table.Name, field.FieldName);
                IEnumerable<IGdRow> rows = _connection.ExecuteReader(query, _table);
                IGdRow row = rows?.FirstOrDefault();
                if (row == null)
                    continue;

                //find geometry format
                GdSqliteGeometryFormat format;
                string geomformat = row.GetAsString("gf");
                if (string.Compare(geomformat, "WKB", StringComparison.OrdinalIgnoreCase) == 0)
                    format = GdSqliteGeometryFormat.WKB;
                else if (string.Compare(geomformat, "WKT", StringComparison.OrdinalIgnoreCase) == 0)
                    format = GdSqliteGeometryFormat.WKT;
                else
                    continue;

                //find geometry type
                long geomtype = row.GetAsInteger("gt");
                OgcGeometryType ogcGeometryType = (OgcGeometryType)geomtype;
                GdGeometryType? geometryType = GdGeometryUtil.ConvertGeometryType(ogcGeometryType);

                //find srid
                long srid = 0;
                if (!row.IsNull("srid"))
                    srid = row.GetAsInteger("srid");

                field.FieldType = GdDataType.Geometry;
                field.GeometryFormat = format;
                field.GeometryType = geometryType;
                field.Srid = (int)srid;
            }
        }

        //+
        public void CreateField(IGdField field)
        {
            if (field.PrimaryKey)
                throw new Exception("sqllite does not support primarykey");

            if (field.NotNull)
                throw new Exception("sqllite does not not null");

            string str = GetDataTypeString(field.FieldType);
            if (!string.IsNullOrEmpty(field.DefaultVal))
                str += " default " + field.DefaultVal;

            string sql = $"ALTER TABLE {_table.Name} ADD COLUMN {field.FieldName} {str}";
            _connection.ExecuteNonQuery(sql);

            //cerate geometry metedata....
            if (field.FieldType == GdDataType.Geometry)
            {
                int ordinal = (int)field.GeometryType + 1;
                sql = "Insert into geometry_columns" +
                      "(f_table_name, f_geometry_column, geometry_type, coord_dimension, srid, geometry_format) " +
                      "values('{0}', '{1}', {2}, {3}, {4}, '{5}')";
                sql = string.Format(sql, _table.Name, field.FieldName, ordinal, 2, field.Srid, "WKB");
                _connection.ExecuteNonQuery(sql);
            }

            RefreshSchema();
        }

        public void DeleteField(IGdField field)
        {
            //todo: Mustafa yazar mısın?
            //domain tablosundan silmeyi unutma....
        }

        private string GetDataTypeString(GdDataType dataType)
        {
            switch (dataType)
            {
                case GdDataType.Blob:
                case GdDataType.Geometry:
                    return "BLOB";
                case GdDataType.Integer:
                    return "INTEGER";
                case GdDataType.Boolean:
                    return "BOOLEAN";
                case GdDataType.Real:
                    return "FLOAT";
                case GdDataType.Date:
                    return "DATE_TIME";
                case GdDataType.String:
                    return "TEXT";
                default:
                    throw new Exception("unsupported data type " + dataType);
            }
        }

        private GdDataType GetDataTypeFromString(string dataTypeStr)
        {
            if (dataTypeStr.ToUpper().Contains("INT"))
                return GdDataType.Integer;
            if (dataTypeStr.ToUpper().Contains("REAL") ||
                dataTypeStr.ToUpper().Contains("FLOA") ||
                dataTypeStr.ToUpper().Contains("DOUB"))
                return GdDataType.Real;
            if (dataTypeStr.ToUpper().Contains("CHAR") ||
                dataTypeStr.ToUpper().Contains("CLOB") ||
                dataTypeStr.ToUpper().Contains("TEXT"))
                return GdDataType.String;
            if (dataTypeStr.ToUpper().Contains("BLOB"))
                return GdDataType.Blob;
            if (dataTypeStr.ToUpper().Contains("DATE"))
                return GdDataType.Date;
            if (dataTypeStr.ToUpper().Contains("TIME"))
                return GdDataType.Date;
            if (dataTypeStr.ToUpper().Contains("BOOL"))
                return GdDataType.Boolean;
            return GdDataType.String;
        }

        public void RefreshSchema()
        {
            _schema = null;
        }
    }
}
