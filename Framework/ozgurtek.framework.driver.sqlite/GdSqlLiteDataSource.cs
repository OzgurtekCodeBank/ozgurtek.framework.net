using ozgurtek.framework.common.Data;
using ozgurtek.framework.core.Data;
using System;
using System.Collections.Generic;
using System.IO;

namespace ozgurtek.framework.driver.sqlite
{
    public class GdSqlLiteDataSource : IGdDomain
    {
        private readonly GdSqlLiteConnection _connection;
        private readonly GdSqliteDomain _domain;

        internal GdSqlLiteDataSource(string connectionString)
        {
            _connection = new GdSqlLiteConnection(connectionString);
            RepairTable(_connection);
            _domain = new GdSqliteDomain(_connection, this);
        }

        public static GdSqlLiteDataSource OpenOrCreate(string connectionString)
        {
            return new GdSqlLiteDataSource(connectionString);
        }

        public static void DeleteSource(string source)
        {
            File.Delete(source);
        }

        private static void RepairTable(GdSqlLiteConnection database)
        {
            //geometry columns
            database.ExecuteNonQuery("create table if not exists geometry_columns(" +
                                     "f_table_name VARCHAR," +
                                     "f_geometry_column VARCHAR," +
                                     "geometry_type INTEGER," +
                                     "coord_dimension INTEGER," +
                                     "srid INTEGER," +
                                     "geometry_format VARCHAR)");

            //index table
            database.ExecuteNonQuery("create table if not exists geometry_index(" +
                                     "pkid int not null," +
                                     "table_column VARCHAR not null," +
                                     "xmin real not null," +
                                     "xmax real not null," +
                                     "ymin real not null," +
                                     "ymax real not null," +
                                     "UNIQUE(pkid, table_column))");

            //index table's index
            database.ExecuteNonQuery("create index if not exists geometry_column_idx on geometry_index" +
                                     "(pkid, table_column, xmin, xmax, ymin, ymax)");


            //domain
            database.ExecuteNonQuery("create table if not exists domains(" +
                                     "domain_id integer unique primary key autoincrement," +
                                     "domain_name text not null unique" +
                                     ")");

            //domain columns
            database.ExecuteNonQuery("create table if not exists domain_columns(" +
                                     "domain_columns_id integer unique primary key autoincrement," +
                                     "f_table_name VARCHAR," +
                                     "f_domain_column VARCHAR," +
                                     "f_domain_name VARCHAR)");

            //coded values
            database.ExecuteNonQuery("create table if not exists coded_values(" +
                                     "coded_value_id integer unique primary key autoincrement, " +
                                     "domain_name VARCHAR not null," +
                                     "code int not null," +
                                     "value text not null," +
                                     "UNIQUE(domain_name, code)" +
                                     ")");

            //spatial ref table
            database.ExecuteNonQuery("create table if not exists spatial_ref_sys(" +
                                     "srid INTEGER UNIQUE, " +
                                     "auth_name TEXT, " +
                                     "auth_srid TEXT, " +
                                     "srtext TEXT)");

            //change table
            database.ExecuteNonQuery("create table if not exists change_table(" +
                                     "change_table_id integer unique primary key autoincrement, " +
                                     "layer_name int not null," +
                                     "fid int not null," +
                                     "change_type int not null," +
                                     "UNIQUE(layer_name, fid)" +
                                     ")");

            //put wgs84 string
            string query = "select count(*) from spatial_ref_sys where srid = 4326";
            int wgs84Exists = database.ExecuteScalar<int>(query);
            if (wgs84Exists == 0)
            {
                query = "Insert into spatial_ref_sys " +
                        " (srid, auth_name, auth_srid, srtext) values " +
                        " ({0}, '{1}', '{2}', '{3}')";
                string wgsString = "GEOGCS[\"WGS 84\",DATUM[\"WGS_1984\",SPHEROID[\"WGS 84\"" +
                                   ",6378137,298.257223563,AUTHORITY[\"EPSG\",\"7030\"]],AUTHORITY[\"EPSG\",\"6326\"]]," +
                                   "PRIMEM[\"Greenwich\",0,AUTHORITY[\"EPSG\",\"8901\"]]," +
                                   "UNIT[\"degree\",0.0174532925199433,AUTHORITY[\"EPSG\",\"9122\"]]," +
                                   "AXIS[\"Latitude\",NORTH],AXIS[\"Longitude\",EAST],AUTHORITY[\"EPSG\",\"4326\"]]";
                string formatedSql = string.Format(query, 4326, "epsg", "4326", wgsString);
                database.ExecuteNonQuery(formatedSql);
            }
        }

        public string ConnectionString
        {
            get { return _connection.ConnectionString; }
        }

        public int TableCount
        {
            get
            {
                string sql = "select count(*) as cnt from sqlite_master where type = 'table'";
                return _connection.ExecuteScalar<int>(sql);
            }
        }

        public IEnumerable<GdSqlLiteTable> GetTable()
        {
            const string sql = "select name from sqlite_master where type = 'table'";
            IEnumerable<IGdRow> rows = _connection.ExecuteReader(sql);
            foreach (IGdRow row in rows)
            {
                string name = row.GetAsString("name");
                yield return new GdSqlLiteTable(this, _connection, name, new GdSqlFilter("SELECT * FROM " + name));
            }
        }

        public GdSqlLiteTable GetTable(string name)
        {
            const string sql = "select name as cnt from sqlite_master where " +
                                "type = 'table' and " +
                                "name = '{0}'";
            string cmdText = string.Format(sql, name);
            object value = _connection.ExecuteScalar<string>(cmdText);
            if (value == null)
                return null;

            return new GdSqlLiteTable(this, _connection, name, new GdSqlFilter("SELECT * FROM " + name));
        }

        public GdSqlLiteTable ExecuteSql(string tableName, IGdFilter filter)
        {
            return new GdSqlLiteTable(this, _connection, tableName, filter);
        }

        public GdSqlLiteTable CreateTable(string name, GdGeometryType? geometryType, int? srid, string options)
        {
            //always create pk
            _connection.ExecuteNonQuery("create table " + name + "(ogc_fid INTEGER unique primary key autoincrement)");

            //geometry field
            GdSqlLiteTable table = new GdSqlLiteTable(this, _connection, name, new GdSqlFilter("SELECT * FROM " + name));
            if (geometryType != null)
            {
                if (!srid.HasValue || srid.Value <= 0)
                    throw new Exception("wrong srid or not defined");

                GdSqliteField field = new GdSqliteField();
                field.FieldName = "geometry";
                field.GeometryType = geometryType.Value;
                field.Srid = srid.Value;
                field.FieldType = GdDataType.Geometry;
                table.CreateField(field);
            }

            return table;
        }

        public void DeleteTable(string name)
        {
            //geometry columns
            string sql = "delete from geometry_columns where f_table_name='{0}'";
            string query = string.Format(sql, name);
            _connection.ExecuteNonQuery(query);

            //delete table
            sql = "drop table {0}";
            query = string.Format(sql, name);
            _connection.ExecuteNonQuery(query);

            //todo:enis delete others...
        }

        public string Name
        {
            get { return "Sqlite Data Source"; }
        }

        public void BeginTransaction()
        {
            _connection.BeginTransaction();
        }

        public void CommitTransaction()
        {
            _connection.CommitTransaction();
        }

        public void RollBackTransaction()
        {
            _connection.RollbackTransaction();
        }

        #region domain

        public IEnumerable<IGdKeyValueSet> GetDomain()
        {
            return _domain.GetDomain();
        }

        public IGdKeyValueSet GetDomain(string domainName)
        {
            return _domain.GetDomain(domainName);
        }

        public void AddDomain(string domainName)
        {
            _domain.AddDomain(domainName);
        }

        public void AddKeyValue(string domainName, long code, string value)
        {
            _domain.AddKeyValue(domainName, code, value);
        }

        public void DeleteDomain(string domainName)
        {
            _domain.DeleteDomain(domainName);
        }

        public void UpdateDomain(string oldValue, string newValue)
        {
            _domain.UpdateDomain(oldValue, newValue);
        }

        public void DeleteKeyValue(string domain, long code)
        {
            _domain.DeleteKeyValue(domain, code);
        }

        public void AddFieldAsDomain(string tableName, string fieldName, string domainName)
        {
            _domain.AddFieldAsDomain(tableName, fieldName, domainName);
        }

        public string GetDomainName(string tableName, string fieldName)
        {
            return _domain.GetDomainName(tableName, fieldName);
        }

        #endregion
    }
}
