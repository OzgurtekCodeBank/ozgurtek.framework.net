using System.Collections.Generic;
using System.Globalization;
using ozgurtek.framework.core.Data;
using ozgurtek.framework.driver.postgres;

namespace ozgurtek.framework.test.winforms.UnitTest.Driver
{
    public class PostgresTest : AbstractDbTableTest
    {
        private readonly string _ds = "185.122.200.110";
        private readonly string _user = "postgres";
        private readonly string _pass = "qg10QQ4ClcAf5ej";
        private readonly string _database = "unit_test";

        private string ConnectionString
        {
            get
            {
                return $"User ID = {_user}; " +
                       "Search Path=public; " +
                       $"Password={_pass};" +
                       $"Host={_ds};" +
                       "Port=5432;" +
                       $"Database={_database};" +
                       "ApplicationName=test";
            }
        }

        public override int GetTableCount()
        {
            GdPgDataSource dataSource = GdPgDataSource.Open(ConnectionString);
            return dataSource.TableCount;
        }

        public override IEnumerable<IGdDbTable> GetTable()
        {
            GdPgDataSource dataSource = GdPgDataSource.Open(ConnectionString);
            return dataSource.GetTable();
        }

        public override IGdDbTable CreateTable(string table)
        {
            GdPgDataSource dataSource = GdPgDataSource.Open(ConnectionString);
            return dataSource.CreateTable(table);
        }

        public override bool CanEditTable
        {
            get { return true; }
        }

        public override void DeleteTable(string table)
        {
            GdPgDataSource dataSource = GdPgDataSource.Open(ConnectionString);
            dataSource.DeleteTable(table);
        }

        public override IGdDbTable GetTable(string tableName)
        {
            GdPgDataSource dataSource = GdPgDataSource.Open(ConnectionString);
            return dataSource.GetTable(tableName);
        }

        public override IGdDbTable ExecuteSql(string tableName, IGdFilter filter)
        {
            GdPgDataSource dataSource = GdPgDataSource.Open(ConnectionString);
            return dataSource.ExecuteSql(tableName, filter);
        }

        public override string GetName()
        {
            return "PG";
        }

        public override string Normalize(string value)
        {
            return value.ToLower(CultureInfo.InvariantCulture).Replace(":", "@");
        }
    }
}
