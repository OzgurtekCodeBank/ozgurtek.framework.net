using System.Collections.Generic;
using System.Globalization;
using ozgurtek.framework.common.Data.Format;
using ozgurtek.framework.core.Data;

namespace ozgurtek.framework.test.winforms.UnitTest.Driver
{
    public class DbMemTableTest : AbstractDbTableTest
    {
        //veriyi postgres'ten memory'e alıyoruz.
        public override IEnumerable<IGdDbTable> GetTable()
        {
            PostgresTest sqlLiteTest = new PostgresTest();

            IEnumerable<IGdDbTable> pgTables = sqlLiteTest.GetTable();
            foreach (IGdDbTable pgTable in pgTables)
            {
                GdMemoryTable memoryTable = GdMemoryTable.LoadFromTable(pgTable);
                memoryTable.Name = pgTable.Name;
                memoryTable.KeyField = pgTable.KeyField;
                memoryTable.GeometryField = pgTable.GeometryField;
                memoryTable.Description = pgTable.Description;
                yield return memoryTable;
            }
        }

        public override int GetTableCount()
        {
            IEnumerable<IGdDbTable> tables = GetTable();
            List<IGdDbTable> memTables = new List<IGdDbTable>(tables);
            return memTables.Count;
        }

        public override IGdDbTable CreateTable(string table)
        {
            GdMemoryTable memoryTable = new GdMemoryTable();
            memoryTable.Name = table;
            return memoryTable;
        }

        public override bool CanEditTable
        {
            get { return true; }
        }

        public override void DeleteTable(string table)
        {
        }

        public override IGdDbTable GetTable(string tableName)
        {
            PostgresTest sqlLiteTest = new PostgresTest();
            IGdDbTable table = sqlLiteTest.GetTable(tableName);
            return GdMemoryTable.LoadFromTable(table);
        }

        public override IGdDbTable ExecuteSql(string tableName, IGdFilter filter)
        {
            PostgresTest sqlLiteTest = new PostgresTest();
            IGdDbTable table = sqlLiteTest.ExecuteSql(tableName, filter);
            return GdMemoryTable.LoadFromTable(table);
        }

        public override string GetName()
        {
            return "MEM";
        }

        public override string Normalize(string value)
        {
            return value.ToLower(CultureInfo.InvariantCulture).Replace(":", "@");
        }
    }
}
