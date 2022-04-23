using System.Collections.Generic;
using NUnit.Framework;
using ozgurtek.framework.common.Data;
using ozgurtek.framework.common.Data.Format;
using ozgurtek.framework.core.Data;

namespace ozgurtek.framework.test.winforms.UnitTest.Driver
{
    [TestFixture]
    public class MemTableTest : AbstractTableTest
    {
        [Test]
        public void CreateTableTest()
        {
            GdMemoryTable table1 = GetMemoryTable(5);

            string geojson1 = table1.ToGeojson(GdGeoJsonSeralizeType.All);

            GdMemoryTable table2 = GdMemoryTable.LoadFromJson(geojson1);
            string geojson2 = table2.ToGeojson(GdGeoJsonSeralizeType.All);

            Assert.AreEqual(table1.RowCount, table2.RowCount);
            Assert.AreEqual(geojson1, geojson2);
        }

        [Test]
        public void TableInTableTest()
        {
            GdMemoryTable mainTable = new GdMemoryTable();
            mainTable.CreateField(new GdField("JSON", GdDataType.String));
            mainTable.CreateField(new GdField("TRANSACTION", GdDataType.Integer));

            //insert table
            GdMemoryTable table = GetMemoryTable(5);
            GdRowBuffer buffer = new GdRowBuffer();
            buffer.Put("JSON", table.ToGeojson(GdGeoJsonSeralizeType.All));
            buffer.Put("TRANSACTION", 0);
            mainTable.Insert(buffer);

            //update table
            table = GetMemoryTable(5);
            buffer = new GdRowBuffer();
            buffer.Put("JSON", table.ToGeojson(GdGeoJsonSeralizeType.All));
            buffer.Put("TRANSACTION", 1);
            mainTable.Insert(buffer);

            //delete table
            table = GetMemoryTable(5);
            buffer = new GdRowBuffer();
            buffer.Put("JSON", table.ToGeojson(GdGeoJsonSeralizeType.All));
            buffer.Put("TRANSACTION", 2);
            mainTable.Insert(buffer);

            string geojson = mainTable.ToGeojson(GdGeoJsonSeralizeType.All);
            GdMemoryTable memoryTable = GdMemoryTable.LoadFromJson(geojson);
            List<IGdRow> rows = new List<IGdRow>(memoryTable.Rows);

            Assert.AreEqual(rows[0].GetAsInteger("TRANSACTION"), 0);
            Assert.AreEqual(GdMemoryTable.LoadFromJson(rows[0].GetAsString("JSON")).RowCount, 5);

            Assert.AreEqual(rows[1].GetAsInteger("TRANSACTION"), 1);
            Assert.AreEqual(GdMemoryTable.LoadFromJson(rows[1].GetAsString("JSON")).RowCount, 5);

            Assert.AreEqual(rows[2].GetAsInteger("TRANSACTION"), 2);
            Assert.AreEqual(GdMemoryTable.LoadFromJson(rows[1].GetAsString("JSON")).RowCount, 5);
        }
    }
}