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

        public static GdPgDataSource Open(string host, string username, string pass, int port, string database, string searchPath)
        {
            NpgsqlConnectionStringBuilder builder = new NpgsqlConnectionStringBuilder();
            builder.Host = host;
            builder.Username = username;
            builder.Password = pass;
            builder.Port = port;
            builder.Database = database;
            builder.SearchPath = searchPath;
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
                string searchPath = string.Empty;
                if (!string.IsNullOrEmpty(CsBuilder.SearchPath))
                    searchPath = $"table_schema='{CsBuilder.SearchPath}' and";

                string sql = $"Select Count(*) From INFORMATION_SCHEMA.tables " +
                             $"Where table_catalog='{CsBuilder.Database}' and {searchPath} " +
                             $"table_name Not In ('spatial_ref_sys', 'geography_columns', 'geometry_columns', 'raster_columns', 'raster_overviews') and " +
                             $"table_type In ('BASE TABLE', 'VIEW')";

                return DbConvert.ToInt32(ExecuteScalar(sql));
            }
        }

        public IEnumerable<GdPgTable> GetTable()
        {
            string searchPath = string.Empty;
            if (!string.IsNullOrEmpty(CsBuilder.SearchPath))
                searchPath = $"table_schema='{CsBuilder.SearchPath}' and";

            string sql = $"Select table_name From INFORMATION_SCHEMA.tables " +
                         $"Where table_catalog='{CsBuilder.Database}' and {searchPath} " +
                         $"table_name Not In ('spatial_ref_sys', 'geography_columns', 'geometry_columns', 'raster_columns', 'raster_overviews') and " +
                         $"table_type In ('BASE TABLE', 'VIEW') " +
                         $"Order By table_name";

            DataTable table = ExecuteTable(sql);
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
            string searchPath = CsBuilder.SearchPath;
            string tableName = name;

            string[] strings = name.Split('.');
            if (strings.Length == 2)
            {
                searchPath = strings[0].Trim();
                tableName = strings[1].Trim();
            }

            if (!string.IsNullOrEmpty(searchPath))
                searchPath = $"table_schema='{searchPath}' and";

            string sql = $"Select table_name From INFORMATION_SCHEMA.tables " +
                         $"Where table_catalog='{CsBuilder.Database}' and {searchPath} " +
                         $"table_name Not In ('spatial_ref_sys', 'geography_columns', 'geometry_columns', 'raster_columns', 'raster_overviews') and " +
                         $"table_type In ('BASE TABLE', 'VIEW') and table_name = '{tableName}'";

            object value = ExecuteScalar(sql);
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
            NpgsqlDataSourceBuilder dataSourceBuilder = new NpgsqlDataSourceBuilder(CsBuilder.ConnectionString);
            dataSourceBuilder.UseNetTopologySuite();
            NpgsqlDataSource dataSource = dataSourceBuilder.Build();
            NpgsqlConnection connection = dataSource.CreateConnection();
            connection.Open();
            return connection;
        }

        public override string Name
        {
            get { return "Postgres Data Source"; }
        }
    }
}
