using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using ozgurtek.framework.common.Data;
using ozgurtek.framework.core.Data;

namespace ozgurtek.framework.driver.sqlserver
{
    public class GdMsSqlDataSource : GdAbstractDbDataSource
    {
        private readonly SqlConnectionStringBuilder _csBuilder;

        internal GdMsSqlDataSource(string source)
        {
            //SqlServerBytesReader reader = new SqlServerBytesReader();
            _csBuilder = new SqlConnectionStringBuilder(source);
        }

        public static GdMsSqlDataSource Open(string source)
        {
            SqlServerTypes.Utilities.LoadNativeAssemblies(AppDomain.CurrentDomain.BaseDirectory);
            return new GdMsSqlDataSource(source);
        }

        public override string ConnectionString
        {
            get { return _csBuilder.ConnectionString; }
        }

        public override IDbConnection GetConnection()
        {
            SqlConnection connection = new SqlConnection(CsBuilder.ConnectionString);
            connection.Open();
            return connection;
        }

        internal SqlConnectionStringBuilder CsBuilder
        {
            get { return _csBuilder; }
        }

        public override string Name
        {
            get { return "MsSqlServer Data Source"; }
        }

        public int TableCount
        {
            get
            {
                string sql = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES " +
                             $"WHERE TABLE_CATALOG='{CsBuilder.InitialCatalog}' AND " +
                             "TABLE_TYPE IN ('BASE TABLE', 'VIEW')";

                return DbConvert.ToInt32(ExecuteScalar(sql));
            }
        }

        public IEnumerable<GdMsSqlTable> GetTable()
        {
            string sql = $"SELECT TABLE_SCHEMA + '.' + TABLE_NAME FROM INFORMATION_SCHEMA.TABLES " +
                         $"WHERE TABLE_CATALOG = '{CsBuilder.InitialCatalog}' AND " +
                         $"TABLE_TYPE IN ('BASE TABLE', 'VIEW') " +
                         $"ORDER BY TABLE_NAME";

            DataTable table = ExecuteTable(sql);
            if (table.Rows.Count == 0)
                yield break;

            foreach (DataRow row in table.Rows)
            {
                string name = DbConvert.ToString(row[0]);
                yield return new GdMsSqlTable(this, name, new GdSqlFilter("SELECT * FROM " + name));
            }
        }

        public GdMsSqlTable GetTable(string name)
        {
            string sql = $"SELECT TABLE_SCHEMA + '.' + TABLE_NAME FROM INFORMATION_SCHEMA.TABLES " +
                         $"WHERE TABLE_CATALOG='{CsBuilder.InitialCatalog}' AND " +
                         $"TABLE_TYPE IN ('BASE TABLE', 'VIEW')";
            object value = ExecuteScalar(sql);
            if (value == null)
                return null;

            return new GdMsSqlTable(this, name, new GdSqlFilter("SELECT * FROM " + name));
        }

        public GdMsSqlTable ExecuteSql(string name, IGdFilter filter)
        {
            return new GdMsSqlTable(this, name, filter);
        }
    }
}