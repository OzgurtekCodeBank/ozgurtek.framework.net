using System;
using System.Collections.Generic;
using System.Globalization;
using ozgurtek.framework.core.Data;
using ozgurtek.framework.driver.oledb;

namespace ozgurtek.framework.test.winforms.UnitTest.Driver
{
    public class OleDbTest : AbstractDbTableTest
    {
        private string ConnectionString
        {
            get
            {
                //please look data folder
                return "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:\\Users\\eniso\\Desktop\\work\\testdata\\unit_test.mdb";
            }
        }

        public override int GetTableCount()
        {
            GdOleDbDataSource dataSource = GdOleDbDataSource.Open(ConnectionString);
            return dataSource.TableCount;
        }

        public override IEnumerable<IGdDbTable> GetTable()
        {
            GdOleDbDataSource dataSource = GdOleDbDataSource.Open(ConnectionString);
            return dataSource.GetTable();
        }

        public override IGdDbTable CreateTable(string table)
        {
            throw new NotSupportedException();
        }

        public override bool CanEditTable
        {
            get { return false; }
        }

        public override void DeleteTable(string table)
        {
            throw new NotSupportedException();
        }

        public override IGdDbTable GetTable(string tableName)
        {
            GdOleDbDataSource dataSource = GdOleDbDataSource.Open(ConnectionString);
            return dataSource.GetTable(tableName);
        }

        public override IGdDbTable ExecuteSql(string tableName, IGdFilter filter)
        {
            GdOleDbDataSource dataSource = GdOleDbDataSource.Open(ConnectionString);
            return dataSource.ExecuteSql(tableName, filter);
        }

        public override string GetName()
        {
            return "OLEDB";
        }

        public override string Normalize(string value)
        {
            return value.ToLower(CultureInfo.InvariantCulture).Replace(":", "@");
        }
    }
}
