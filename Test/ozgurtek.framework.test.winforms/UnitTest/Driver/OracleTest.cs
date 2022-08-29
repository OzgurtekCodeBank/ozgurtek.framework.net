using System.Collections.Generic;
using System.Globalization;
using ozgurtek.framework.core.Data;
using ozgurtek.framework.driver.oracle;

namespace ozgurtek.framework.test.winforms.UnitTest.Driver
{
    public class OracleTest : AbstractDbTableTest
    {
        private readonly string _ds = "185.122.200.110:1521/ORCL";
        private readonly string _user = "UNIT_TEST";
        private readonly string _pass = "12";

        private string ConnectionString
        {
            get
            {
                return $"Data Source={_ds};" +
                       $"User Id={_user};" +
                       $"Password={_pass};" +
                       "Pooling=True;" +
                       "Min Pool Size=1;" +
                       "Max Pool Size=100;" +
                       "Incr Pool Size=5;" +
                       "Decr Pool Size=1;" +
                       "Connection Lifetime=0;" +
                       "Connection Timeout=15;" +
                       "Self Tuning=True;";
            }
        }

        public override int GetTableCount()
        {
            GdOracleDataSource dataSource = GdOracleDataSource.Open(ConnectionString);
            return dataSource.TableCount;
        }

        public override IEnumerable<IGdDbTable> GetTable()
        {
            GdOracleDataSource dataSource = GdOracleDataSource.Open(ConnectionString);
            return dataSource.GetTable();
        }

        public override IGdDbTable CreateTable(string table)
        {
            return null;
        }

        public override bool CanEditTable
        {
            get { return false; }
        }

        public override void DeleteTable(string table)
        {
            throw new System.NotImplementedException();
        }

        public override IGdDbTable GetTable(string tableName)
        {
            GdOracleDataSource dataSource = GdOracleDataSource.Open(ConnectionString);
            return dataSource.GetTable(tableName);
        }

        public override IGdDbTable ExecuteSql(string tableName, IGdFilter filter)
        {
            GdOracleDataSource dataSource = GdOracleDataSource.Open(ConnectionString);
            return dataSource.ExecuteSql(tableName, filter);
        }

        public override string GetName()
        {
            return "ORACLE";
        }

        public override string Normalize(string value)
        {
            return value.ToUpper(CultureInfo.InvariantCulture).Replace("@", ":");
        }
    }
}