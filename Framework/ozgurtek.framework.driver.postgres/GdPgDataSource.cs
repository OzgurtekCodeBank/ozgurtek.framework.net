using Npgsql;
using ozgurtek.framework.common.Data;
using ozgurtek.framework.core.Data;
using System.Collections.Generic;
using System.Data;

namespace ozgurtek.framework.driver.postgres
{
    public class GdPgDataSource : GdAbstractDbDataSource
    {
        private readonly NpgsqlConnectionStringBuilder _csBuilder;

        internal GdPgDataSource(string source)
        {
            _csBuilder = new NpgsqlConnectionStringBuilder(source);
        }

        public static GdPgDataSource Open(string source)
        {
            return new GdPgDataSource(source);
        }

        public static GdPgDataSource Open(string host, string username, string pass, int port, string database)
        {
            NpgsqlConnectionStringBuilder builder = new NpgsqlConnectionStringBuilder();
            builder.Host = host;
            builder.Username = username;
            builder.Password = pass;
            builder.Port = port;
            builder.Database = database;
            builder.SearchPath = "public";
            builder.ApplicationName = "ozgurtek.framework.driver.postgres";
            string connectionString = builder.ConnectionString;
            return Open(connectionString);
        }

        public override string ConnectionString
        {
            get { return _csBuilder.ConnectionString; }
        }

        public int TableCount
        {
            get
            {
                string sql = $"Select Count(*) From INFORMATION_SCHEMA.tables " +
                             $"Where table_catalog='{CsBuilder.Database}' and table_schema='{CsBuilder.SearchPath}' and " +
                             $"table_name Not In ('spatial_ref_sys', 'geography_columns', 'geometry_columns', 'raster_columns', 'raster_overviews') and " +
                             $"table_type In ('BASE TABLE', 'VIEW')";

                return DbConvert.ToInt32(ExecuteScalar(sql));
            }
        }

        public IEnumerable<GdPgTable> GetTable()
        {
            string sql = $"Select table_name From INFORMATION_SCHEMA.tables " +
                         $"Where table_catalog='{CsBuilder.Database}' and table_schema='{CsBuilder.SearchPath}' and " +
                         $"table_name Not In ('spatial_ref_sys', 'geography_columns', 'geometry_columns', 'raster_columns', 'raster_overviews') and " +
                         $"table_type In ('BASE TABLE', 'VIEW') " +
                         $"Order By table_name";

            string cmdText = string.Format(sql, CsBuilder.SearchPath);
            DataTable table = ExecuteTable(cmdText);
            if (table.Rows.Count == 0)
                yield break;

            foreach (DataRow row in table.Rows)
            {
                string name = DbConvert.ToString(row[0]);
                yield return new GdPgTable(this, name, new GdSqlFilter("SELECT * FROM " + name));
            }
        }

        public GdPgTable GetTable(string name)
        {
            string sql = $"Select table_name From INFORMATION_SCHEMA.tables " +
                         $"Where table_catalog='{CsBuilder.Database}' and table_schema='{CsBuilder.SearchPath}' and " +
                         $"table_name Not In ('spatial_ref_sys', 'geography_columns', 'geometry_columns', 'raster_columns', 'raster_overviews') and " +
                         $"table_type In ('BASE TABLE', 'VIEW')";

            string cmdText = string.Format(sql, CsBuilder.SearchPath, name);
            object value = ExecuteScalar(cmdText);
            if (value == null)
                return null;

            return new GdPgTable(this, name, new GdSqlFilter("SELECT * FROM " + name));
        }

        public GdPgTable ExecuteSql(string name, IGdFilter filter)
        {
            return new GdPgTable(this, name, filter);
        }

        public GdPgTable CreateTable(string tableName)
        {
            string sql = $"CREATE TABLE IF NOT EXISTS {tableName}()";
            ExecuteScalar(sql);
            return GetTable(tableName);
        }

        public void DeleteTable(string tableName)
        {
            string sql = $"drop table {tableName}";
            ExecuteScalar(sql);
        }

        internal NpgsqlConnectionStringBuilder CsBuilder
        {
            get { return _csBuilder; }
        }

        public override IDbConnection GetConnection()
        {
            NpgsqlConnection connection = new NpgsqlConnection(CsBuilder.ConnectionString);
            connection.Open();
            connection.TypeMapper.UseNetTopologySuite();
            return connection;
        }

        public override string Name
        {
            get { return "Postgres Data Source"; }
        }
    }
}
