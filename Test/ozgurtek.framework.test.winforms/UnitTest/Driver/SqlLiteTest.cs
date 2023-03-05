using System.Collections.Generic;
using System.Globalization;
using ozgurtek.framework.core.Data;
using ozgurtek.framework.driver.sqlite;

namespace ozgurtek.framework.test.winforms.UnitTest.Driver
{
    public class SqlLiteTest : AbstractDbTableTest
    {
        private string ConnectionString
        {
            get
            {
                //please look data folder
                return @"C:\data\work\unittest\unit_test.sqlite";
            }
        }

        public override int GetTableCount()
        {
            GdSqlLiteDataSource dataSource = GdSqlLiteDataSource.OpenOrCreate(ConnectionString);
            return dataSource.TableCount;
        }

        public override IEnumerable<IGdDbTable> GetTable()
        {
            GdSqlLiteDataSource dataSource = GdSqlLiteDataSource.OpenOrCreate(ConnectionString);
            return dataSource.GetTable();
        }

        public override IGdDbTable CreateTable(string table)
        {
            GdSqlLiteDataSource dataSource = GdSqlLiteDataSource.OpenOrCreate(ConnectionString);
            return dataSource.CreateTable(table, null, null, null);
        }

        public override bool CanEditTable
        {
            get { return true; }
        }

        public override void DeleteTable(string table)
        {
            GdSqlLiteDataSource dataSource = GdSqlLiteDataSource.OpenOrCreate(ConnectionString);
            dataSource.DeleteTable(table);
        }

        public override IGdDbTable GetTable(string tableName)
        {
            GdSqlLiteDataSource dataSource = GdSqlLiteDataSource.OpenOrCreate(ConnectionString);
            return dataSource.GetTable(tableName);
        }

        public override IGdDbTable ExecuteSql(string tableName, IGdFilter filter)
        {
            GdSqlLiteDataSource dataSource = GdSqlLiteDataSource.OpenOrCreate(ConnectionString);
            return dataSource.ExecuteSql(tableName, filter);
        }

        public override string GetName()
        {
            return "SQLITE";
        }

        public override string Normalize(string value)
        {
            return value.ToLower(CultureInfo.InvariantCulture).Replace(":", "@");
        }
    }
}