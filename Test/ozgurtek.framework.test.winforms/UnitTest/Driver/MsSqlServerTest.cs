using System.Collections.Generic;
using System.Globalization;
using ozgurtek.framework.core.Data;
using ozgurtek.framework.driver.sqlserver;

namespace ozgurtek.framework.test.winforms.UnitTest.Driver
{
    public class MsSqlServerTest : AbstractDbTableTest
    {
        private readonly string _ds = "xyz";
        private readonly string _user = "xyz";
        private readonly string _pass = "xyz";
        private readonly string _catalog = "xyz";

        private string ConnectionString
        {
            get
            {
                return $"Data Source = {_ds}; " +
                       "Integrated Security = False; " +
                       $"User Id = {_user}; " +
                       $"Password = {_pass}; " +
                       $"Initial Catalog = {_catalog}; " +
                       "Pooling = True; " +
                       "Min Pool Size=0; " +
                       "Max Pool Size=100; " +
                       "Connection Timeout = 15; " +
                       "MultipleActiveResultSets = False;";
            }
        }

        public override int GetTableCount()
        {
            GdMsSqlDataSource dataSource = GdMsSqlDataSource.Open(ConnectionString);
            return dataSource.TableCount;
        }

        public override IEnumerable<IGdDbTable> GetTable()
        {
            GdMsSqlDataSource dataSource = GdMsSqlDataSource.Open(ConnectionString);
            return dataSource.GetTable();
        }

        public override IGdDbTable CreateTable(string table)
        {
            throw new System.NotImplementedException();
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
            GdMsSqlDataSource dataSource = GdMsSqlDataSource.Open(ConnectionString);
            return dataSource.GetTable(tableName);
        }

        public override IGdDbTable ExecuteSql(string tableName, IGdFilter filter)
        {
            GdMsSqlDataSource dataSource = GdMsSqlDataSource.Open(ConnectionString);
            return dataSource.ExecuteSql(tableName, filter);
        }

        public override string GetName()
        {
            return "MSSQL";
        }

        public override string Normalize(string value)
        {
            return value.ToLower(CultureInfo.InvariantCulture).Replace(":", "@");
        }
    }
}
