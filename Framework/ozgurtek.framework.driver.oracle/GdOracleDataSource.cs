using ozgurtek.framework.common.Data;
using ozgurtek.framework.core.Data;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using NetTopologySuite.IO;
using Oracle.DataAccess.Client;
using ozgurtek.framework.driver.oracle.NetTopologySuit.IO.Oracle;


namespace ozgurtek.framework.driver.oracle
{
    public class GdOracleDataSource : GdAbstractDbDataSource
    {
        private readonly OracleConnectionStringBuilder _csBuilder;

        internal GdOracleDataSource(string source)
        {
            //bu alttaki satırı silme dll'in ayağa kalkması için...
            OracleGeometryReader reader = new OracleGeometryReader();

            _csBuilder = new OracleConnectionStringBuilder(source);
        }

        public static GdOracleDataSource Open(string source)
        {
            return new GdOracleDataSource(source);
        }

        public override string ConnectionString
        {
            get { return _csBuilder.ConnectionString; }
        }

        public int TableCount
        {
            get
            {
                string sql =
                    $"SELECT COUNT(*) FROM " +
                    $"(SELECT TABLE_NAME AS NAME FROM ALL_TABLES WHERE OWNER='{_csBuilder.UserID}' " +
                    $"AND SECONDARY='N' UNION ALL SELECT VIEW_NAME AS NAME FROM ALL_VIEWS WHERE OWNER='{_csBuilder.UserID}')";
                return DbConvert.ToInt32(ExecuteScalar(sql));
            }
        }

        public IEnumerable<GdOracleTable> GetTable()
        {
            string sql =
                $"SELECT TABLE_NAME AS NAME FROM ALL_TABLES WHERE OWNER='{_csBuilder.UserID}' " +
                $"AND SECONDARY='N' UNION ALL SELECT VIEW_NAME AS NAME FROM ALL_VIEWS WHERE OWNER='{_csBuilder.UserID}'";

            DataTable table = ExecuteTable(sql);
            if (table.Rows.Count == 0)
                yield break;

            foreach (DataRow row in table.Rows)
            {
                string name = DbConvert.ToString(row[0]);
                yield return new GdOracleTable(this, name, new GdSqlFilter("SELECT * FROM " + name));
            }
        }

        public GdOracleTable GetTable(string name)
        {
            return new GdOracleTable(this, name, new GdSqlFilter("SELECT * FROM " + name));
        }

        public GdOracleTable ExecuteSql(string name, IGdFilter filter)
        {
            return new GdOracleTable(this, name, filter);
        }

        internal OracleConnectionStringBuilder CsBuilder
        {
            get { return _csBuilder; }
        }

        public override IDbConnection GetConnection()
        {
            OracleConnection connection = new OracleConnection(CsBuilder.ConnectionString);
            connection.Open();
            return connection;
        }

        public override string Name
        {
            get { return "Oracle Data Source"; }
        }
    }
}