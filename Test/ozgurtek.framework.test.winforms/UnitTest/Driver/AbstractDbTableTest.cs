using System;
using System.Collections.Generic;
using System.Linq;
using NetTopologySuite.Geometries;
using NUnit.Framework;
using ozgurtek.framework.common;
using ozgurtek.framework.common.Data;
using ozgurtek.framework.common.Data.Format;
using ozgurtek.framework.core.Data;
using Assert = NUnit.Framework.Assert;
using Point = NetTopologySuite.Geometries.Point;

namespace ozgurtek.framework.test.winforms.UnitTest.Driver
{
    [TestFixture]
    public abstract class AbstractDbTableTest : AbstractTableTest
    {
        public abstract int GetTableCount();
        public abstract IEnumerable<IGdDbTable> GetTable();
        public abstract IGdDbTable CreateTable(string table);
        public abstract bool CanEditTable { get; }
        public abstract void DeleteTable(string table);
        public abstract IGdDbTable GetTable(string tableName);
        public abstract IGdDbTable ExecuteSql(string tableName, IGdFilter filter);
        public abstract string GetName();

        public abstract string Normalize(string value);

        /**
         * Veritabanındaki tablo sayısını öğrenmek
         */
        [Test]
        public void TableCountTest()
        {
            int value = GetTableCount();
            Assert.GreaterOrEqual(value, 1);
        }

        /**
        * Veritabanındaki tüm tablo isimlerini almak
        */
        [Test]
        public void GetTablesTest()
        {
            IEnumerable<IGdTable> tables = GetTable();
            List<IGdTable> clone = new List<IGdTable>(tables);
            Assert.GreaterOrEqual(clone.Count, 1);
        }

        /**
         * tablo adı ile bir tabloyu almak
         */
        [Test]
        public void GetTableTest()
        {
            IGdTable table = GetTable(Normalize("MAHALLE"));
            Assert.IsNotNull(table);
        }


        /**
         * bir value ile tablo oluşturuyoruz
         * sonra bu tabloya bir filitre koyuyoruz
         * tablodaki row sayısını alıyoruz.
         */
        [Test]
        public void GetQueryTable()
        {
            GdSqlFilter baseFilter = new GdSqlFilter(Normalize("SELECT * FROM PARSEL WHERE OBJECTID > :p1"));
            baseFilter.Add("p1", 5);
            IGdDbTable table = ExecuteSql(Normalize("TEST_PARSEL"), baseFilter);

            GdSqlFilter sqlFilter = new GdSqlFilter(Normalize("OBJECTID < :p2"));
            sqlFilter.Add("p2", 200);
            table.SqlFilter = sqlFilter;

            long tableRowCount = table.RowCount;
            Assert.GreaterOrEqual(tableRowCount, 1);
        }

        /**
         * bir tablonun fieldlarını öğrenmek.
         */
        [Test]
        public void GetSchema()
        {
            IGdTable table = GetTable(Normalize("PARSEL"));

            List<string> fields = new List<string>();
            IGdSchema schema = table.Schema;
            foreach (IGdField field in schema.Fields)
            {
                string fieldName = field.FieldName;
                string fieldType = field.FieldType.ToString();
                fields.Add($"{fieldName}-{fieldType}");
            }

            Assert.GreaterOrEqual(fields.Count, 1);
        }

        /**
        * bir tablonun fieldlarını öğrenmek.
        */
        [Test]
        public void GetTableTest2()
        {
            IGdTable table = GetTable(Normalize("TEMP"));

            Assert.AreEqual(table.Schema.Count, 9);
            Assert.AreEqual(table.Schema.GetFieldByName(Normalize("OBJECTID")).FieldType, GdDataType.Integer);
            Assert.AreEqual(table.Schema.GetFieldByName(Normalize("STR_FIELD")).FieldType, GdDataType.String);
            Assert.AreEqual(table.Schema.GetFieldByName(Normalize("DOUBLE_FIELD")).FieldType, GdDataType.Real);
            Assert.AreEqual(table.Schema.GetFieldByName(Normalize("BLOB_FIELD")).FieldType, GdDataType.Blob);
            Assert.AreEqual(table.Schema.GetFieldByName(Normalize("SHAPE")).FieldType, GdDataType.Geometry);
            Assert.AreEqual(table.Schema.GetFieldByName(Normalize("INT_FIELD")).FieldType, GdDataType.Integer);

            if (GetName() != "SQLITE")
                Assert.AreEqual(table.Schema.GetFieldByName(Normalize("DATE_FIELD")).FieldType,
                    GdDataType.Date); //todo: sqlite'da datetime yok.

            if (GetName() != "ORACLE" && GetName() != "SQLITE")
                Assert.AreEqual(table.Schema.GetFieldByName(Normalize("BOOL_FIELD")).FieldType,
                    GdDataType.Boolean); //todo: oracle'da ve boolean yok.

            if (GetName() != "SQLITE")
                Assert.AreEqual(table.Schema.GetFieldByName(Normalize("GEOM_FIELD")).FieldType, GdDataType.Geometry);
        }

        /**
         * Satırlar içerinde dön...
         * select, limit, offset, order by test
         */
        [Test]
        public void GetRows()
        {
            IGdDbTable table = GetTable(Normalize("MAHALLE"));
            table.ColumnFilter = Normalize("OBJECTID, SHAPE");
            table.OrderBy = Normalize("OBJECTID DESC");
            table.Offset = 8;
            table.Limit = 4;

            List<string> fields = new List<string>();
            IEnumerable<IGdRow> rows = table.Rows;
            foreach (IGdRow row in rows)
            {
                if (row.IsNull(Normalize("SHAPE")))
                    continue;

                Geometry geometry = row.GetAsGeometry(Normalize("SHAPE"));
                string text = geometry.ToText();

                int integer = row.GetAsInteger(Normalize("OBJECTID"));
                fields.Add($"{integer}-{text}");
            }

            Assert.AreEqual(fields.Count, 4);
        }

        /**
         * KeyField verilen bir satırdaki tüm değerleri almak:
         */
        [Test]
        public void FindRowTest()
        {
            IGdDbTable table = GetTable(Normalize("PARSEL"));
            table.KeyField = Normalize(Normalize("OBJECTID"));
            table.GeometryField = Normalize("SHAPE");
            IGdRow row = table.FindRow(5);

            List<string> values = new List<string>();
            IGdSchema schema = table.Schema;
            foreach (IGdField field in schema.Fields)
            {
                string fieldName = field.FieldName;
                if (row.IsNull(fieldName))
                    continue;

                string words = $"{fieldName} : {row.GetAsString(fieldName)}";
                values.Add(words);
            }

            Assert.GreaterOrEqual(values.Count, 1);
        }

        /**
         * Tablodaki bir alandaki distinct değerleri alma
         */
        [Test]
        public void DistinctTest()
        {
            GdSqlFilter filter =
                new GdSqlFilter(Normalize("SELECT * FROM PARSEL WHERE OBJECTID > 10 AND OBJECTID < 1000"));
            IGdDbTable table = ExecuteSql("PARSEL_TEST", filter);

            table.KeyField = Normalize(Normalize("OBJECTID"));
            table.GeometryField = Normalize("SHAPE");

            List<string> values = new List<string>();
            IEnumerable<object> distinctValues = table.GetDistinctValues(Normalize("ACIKLAMA"));
            foreach (object value in distinctValues)
                values.Add(value.ToString());

            Assert.GreaterOrEqual(values.Count, 1);
        }

        /**
        * Memory Tablosuna Dönüştürme
        */
        [Test]
        public void CreateMemTable()
        {
            if (GetName() == "SQLITE")
                return; //todo: sqlite bilinen bir arıza var şema ile ilgili, query layerda şema meselesi sıkıntı olıyor.

            GdSqlFilter filter =
                new GdSqlFilter(Normalize("SELECT * FROM MAHALLE WHERE OBJECTID > 10 AND OBJECTID < 1000"));
            IGdDbTable table = ExecuteSql("MAHALLE_TEST", filter);

            table.KeyField = Normalize(Normalize("OBJECTID"));
            table.GeometryField = Normalize("SHAPE");

            GdMemoryTable memoryTable = GdMemoryTable.LoadFromTable(table);

            Assert.Greater(table.RowCount, 1);
            Assert.Greater(memoryTable.RowCount, 1);
        }

        /*
         * Verilen bir nokta ile kesişen parseli bulmak..
         */
        [Test]
        public void SpatialFilterTest()
        {
            double x = 4250200;
            double y = 514305;

            IGdDbTable table = GetTable(Normalize("MAHALLE"));
            table.KeyField = Normalize("OBJECTID");
            table.GeometryField = Normalize("SHAPE");

            GeometryFactory factory = GdFactoryFinder.Instance.GeometryServices.CreateGeometryFactory(5253);
            Point point = factory.CreatePoint(new Coordinate(y, x));
            GdGeometryFilter geometryFilter = new GdGeometryFilter(point, GdSpatialRelation.Intersects);
            table.GeometryFilter = geometryFilter;

            Assert.GreaterOrEqual(table.RowCount, 0);
        }

        /**
         * Mahalleye giren yapıları almak
         */
        [Test]
        public void SpatialFilterByPolygonTest()
        {
            IGdDbTable parsel = GetTable(Normalize("MAHALLE"));
            IGdFilter filter = new GdSqlFilter(Normalize("OBJECTID = 5"));
            parsel.SqlFilter = filter;
            IGdRow parselRow = parsel.Rows.FirstOrDefault();
            Geometry mahalleGeom = parselRow.GetAsGeometry(Normalize("SHAPE"));

            IGdDbTable parselTable = GetTable(Normalize("PARSEL"));
            parselTable.GeometryField = Normalize("SHAPE");
            parselTable.GeometryFilter = new GdGeometryFilter(mahalleGeom, GdSpatialRelation.Within);
            parselTable.SqlFilter = new GdSqlFilter("tip = 1");

            Assert.Greater(parselTable.RowCount, 1);
        }

        /**
        * Katmanın kapsadığı alanı almak
        */
        [Test]
        public void GetEnvelopeTest()
        {
            IGdDbTable parsel = GetTable(Normalize("ILCE"));
            Envelope envelope = parsel.Envelope;
            Assert.NotNull(envelope);
        }

        /**
        * Katmanın geometrisini almak
        */
        [Test]
        public void GetGeometry()
        {
            List<string> values = new List<string>();
            IEnumerable<IGdDbTable> tables = GetTable();
            foreach (IGdDbTable table in tables)
            {
                string tableName = table.Name;
                var geometryType = !string.IsNullOrWhiteSpace(table.GeometryField)
                    ? table.GeometryType.ToString()
                    : "sözel";

                values.Add($"{tableName}-{geometryType}");
            }

            Assert.Greater(values.Count, 1);
        }

        /// <summary>
        /// katmanın srid'sini almak
        /// </summary>
        [Test]
        public void GetSrid()
        {
            List<string> values = new List<string>();
            IEnumerable<IGdDbTable> tables = GetTable();
            foreach (IGdDbTable table in tables)
            {
                string tableName = table.Name;
                var srid = !string.IsNullOrWhiteSpace(table.GeometryField) ? table.Srid.ToString() : "sözel";

                values.Add($"{tableName}-{srid}");
            }

            Assert.Greater(values.Count, 1);
        }

        /// <summary>
        /// tabloya insert
        /// </summary>
        [Test]
        public void InsertRowTest()
        {
            IGdDbTable table = GetTable(Normalize(Normalize("TEMP")));
            if (!table.CanEditRow)
                return;

            table.KeyField = Normalize(Normalize("OBJECTID"));
            long id  = table.Insert(GetOneRow());
            Assert.Greater(id, 0);
        }

        /// <summary>
        /// tabloya insert
        /// </summary>
        [Test]
        public void UpdateRowTest()
        {
            IGdDbTable table = GetTable(Normalize(Normalize("TEMP")));
            table.KeyField = Normalize(Normalize("OBJECTID"));
            long id = table.Insert(GetOneRow());

            GdRowBuffer buffer = new GdRowBuffer();

            //no datatype def with null
            buffer.Put(Normalize("OBJECTID"), id);
            buffer.PutNull(Normalize("STR_FIELD"));
            buffer.PutNull(Normalize("DOUBLE_FIELD"));
            buffer.PutNull(Normalize("DATE_FIELD"));
            buffer.PutNull(Normalize("BOOL_FIELD"));
            buffer.Put(Normalize("SHAPE"), null, GdDataType.Geometry); //oracle'da tipi vermek gerek
            buffer.Put(Normalize("GEOM_FIELD"), null, GdDataType.Geometry); //oracle'da tipi vermek gerek
            buffer.Put(Normalize("BLOB_FIELD"), null, GdDataType.Blob); //mssql'da tipi vermek gerek
            long update = table.Update(buffer);
            Assert.GreaterOrEqual(update, 0);

            //datatype def with null
            buffer.Put(Normalize("OBJECTID"), id, GdDataType.Integer);
            buffer.Put(Normalize("STR_FIELD"), null, GdDataType.String);
            buffer.Put(Normalize("DOUBLE_FIELD"), null, GdDataType.Real);
            buffer.Put(Normalize("DATE_FIELD"), null, GdDataType.Date);
            buffer.Put(Normalize("BOOL_FIELD"), null, GdDataType.Boolean);
            buffer.Put(Normalize("SHAPE"), null, GdDataType.Geometry);
            buffer.Put(Normalize("GEOM_FIELD"), null, GdDataType.Geometry);
            buffer.Put(Normalize("BLOB_FIELD"), null, GdDataType.Blob);
            update = table.Update(buffer);
            Assert.GreaterOrEqual(update, 0);

            //data type with value
            buffer.Put(Normalize("OBJECTID"), id, GdDataType.Integer);
            buffer.Put(Normalize("STR_FIELD"), Guid.NewGuid().ToString(), GdDataType.String);
            buffer.Put(Normalize("DOUBLE_FIELD"), GetDouble(), GdDataType.Real);
            buffer.Put(Normalize("DATE_FIELD"), DateTime.Now, GdDataType.Date);
            buffer.Put(Normalize("BOOL_FIELD"), false, GdDataType.Boolean);
            buffer.Put(Normalize("SHAPE"), GetGeoemetry(), GdDataType.Geometry);
            buffer.Put(Normalize("GEOM_FIELD"), GetGeoemetry(), GdDataType.Geometry);
            buffer.Put(Normalize("BLOB_FIELD"), GetBlob(), GdDataType.Blob);
            update = table.Update(buffer);
            Assert.GreaterOrEqual(update, 0);
        }

        /// <summary>
        /// tabloya delete
        /// </summary>
        [Test]
        public void DeleteRowTest()
        {
            IGdDbTable table = GetTable(Normalize(Normalize("TEMP")));
            table.KeyField = Normalize(Normalize("OBJECTID"));
            long id = table.Insert(GetOneRow());
            long delete = table.Delete(id);
            Assert.GreaterOrEqual(delete, 0);
        }

        /// <summary>
        /// tabloya truncate
        /// </summary>
        [Test]
        public void TruncateTest()
        {
            IGdDbTable table = GetTable(Normalize(Normalize("TEMP")));
            table.Truncate();
        }

        /// <summary>
        /// tablo silmek
        /// </summary>
        [Test]
        public void CreateDeleteTableTest()
        {
            if (!CanEditTable)
                return;

            CreateTable("testtable");
            DeleteTable("testtable");
        }

        /// <summary>
        /// FTS sorgusu...
        /// </summary>
        [Test]
        public void FtsTest()
        {
            if (GetName() != "PG")
                return; //sadece pg de çalışıyor....

            IGdDbTable table = GetTable("numarataj");

            GdPgFtsBuilder ftsBuilder = new GdPgFtsBuilder();
            ftsBuilder.SearchKey = "16 3";
            ftsBuilder.Add("kapino");
            ftsBuilder.Add("tip");

            GdSqlFilter filter = new GdSqlFilter(ftsBuilder.BuildWhereClause());
            table.SqlFilter = filter;
            long tableRowCount = table.RowCount;
            Assert.GreaterOrEqual(tableRowCount, 0);
        }
    }
}