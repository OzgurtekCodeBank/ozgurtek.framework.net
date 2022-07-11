using System;
using System.Collections.Generic;
using System.Linq;
using NetTopologySuite.Geometries;
using NUnit.Framework;
using ozgurtek.framework.common.Data;
using ozgurtek.framework.common.Data.Format;
using ozgurtek.framework.core.Data;

namespace ozgurtek.framework.test.winforms.UnitTest.Driver
{
    [TestFixture]
    public class ServerDataSourceTest : AbstractTableTest
    {
        //username:password:applicationid
        private readonly string _credential = "enisozgur:Ozgurtek/2019:30CB5074-897E-426E-B61A-64B057FB64A7";
        private readonly Uri _uri = new Uri("http://localhost:57017/GdSpatialService.svc");

        private GdServerDataSource CreateNewDataSource
        {
            get
            {
                GdServerDataSource dataSource = new GdServerDataSource(_uri, _credential);
                return dataSource;
            }
        }

        [Test]
        public void CheckUserByMagicPasswordTest()
        {
            bool credential = CreateNewDataSource.ValidateCredential();
            Assert.AreEqual(credential, true);
        }

        
        [Test]
        public void GetTableTest()
        {
            GdMemoryTable memoryTable = CreateNewDataSource.GetTable("unit_test:v1_get_all_il");
            Assert.AreEqual(memoryTable.RowCount, 1);

            memoryTable = CreateNewDataSource.GetTable("unit_test:v1_get_all_ilce");
            Assert.AreEqual(memoryTable.RowCount, 1);

            memoryTable = CreateNewDataSource.GetTable("unit_test:v1_get_all_mahalle", 3,5);
            Assert.Greater(memoryTable.RowCount, 1);
        }


        [Test]
        public void GetGeometryTest()
        {
            GdMemoryTable memoryTable = CreateNewDataSource.GetTable("unit_test:v1_get_all_il");
            IGdRow row = memoryTable.Rows.FirstOrDefault();
            Geometry geometry = row.GetAsGeometry("shape");
            Assert.NotNull(geometry);
        }
        

        [Test]
        public void GetTableByParamTest()
        {
            GdServerDataSource serverDataSource = CreateNewDataSource;
            serverDataSource.AddParameter("p1", "2");//limit
            serverDataSource.AddParameter("p2", "20");//offset
            GdMemoryTable memoryTable = serverDataSource.GetTable("unit_test:v1_get_bagimisizbolum");
            Assert.AreEqual(memoryTable.RowCount, 2);
        }

        [Test]
        public void GetTableByParamTest2()
        {
            GdServerDataSource serverDataSource = CreateNewDataSource;
            GdMemoryTable memoryTable = serverDataSource.GetTable("unit_test:v1_get_bagimisizbolum_all", 2, 20);
            Assert.AreEqual(memoryTable.RowCount, 2);
        }

        [Test]
        public void GetTempTableTest()
        {
            GdMemoryTable memoryTable = CreateNewDataSource.GetTable("unit_test:v1_get_all_temp_table");
            Assert.Greater(memoryTable.RowCount, 0);
        }

        [Test]
        public void InsertRowTest()
        {
            GdMemoryTable memoryTable = GetMemoryTable(10);
            GdServerDataSource dataSource = CreateNewDataSource;
            GdMemoryTable commit = dataSource.Commit("unit_test:v1_transaction_temp_table", memoryTable, GdServerDataSource.CommitType.Insert);
            IGdRow row = commit.Rows.FirstOrDefault();
            long singleRow = row.GetAsInteger("objectid");
            Assert.Greater(singleRow, 0);
        }

        [Test]
        public void UpdateRowTest()
        {
            GdServerDataSource dataSource = CreateNewDataSource;

            //insert
            GdMemoryTable memoryTable = GetMemoryTable(1);
            dataSource.Commit("unit_test:v1_transaction_temp_table", memoryTable, GdServerDataSource.CommitType.Insert);

            //update
            GdMemoryTable allTable = dataSource.GetTable("unit_test:v1_get_all_temp_table");
            GdMemoryTable resultTable = dataSource.Commit("unit_test:v1_transaction_temp_table", allTable, GdServerDataSource.CommitType.Update);

            Assert.Greater(resultTable.RowCount, 0);
        }

        [Test]
        public void DeleteRowTest()
        {
            GdServerDataSource dataSource = CreateNewDataSource;

            //insert
            GdMemoryTable memoryTable = GetMemoryTable(10);
            GdMemoryTable result = dataSource.Commit("unit_test:v1_transaction_temp_table", memoryTable, GdServerDataSource.CommitType.Insert);

            //delete
            GdMemoryTable deleteTable = new GdMemoryTable();
            deleteTable.CreateField(new GdField("objectid", GdDataType.Integer));
            foreach (IGdRow row in result.Rows)
            {
                int integer = row.GetAsInteger("objectid");
                GdRowBuffer buffer = new GdRowBuffer();
                buffer.Put("objectid", integer);
                deleteTable.Insert(buffer);
            }
            GdMemoryTable resultTable = dataSource.Commit("unit_test:v1_transaction_temp_table", deleteTable, GdServerDataSource.CommitType.Delete);

            Assert.Greater(resultTable.RowCount, 0);
        }

        [Test]
        public void CommitMultiTableTest()
        {
            GdMemoryTable table1 = GetMemoryTable(50);
            table1.Name = "unit_test: v1_transaction_temp_table";

            GdMemoryTable table2 = GetMemoryTable(10);
            table2.Name = "unit_test: v1_transaction_temp_table";

            GdServerDataSource.TableSet tableSet = new GdServerDataSource.TableSet();
            tableSet.Add(table1, GdServerDataSource.CommitType.Insert);
            tableSet.Add(table2, GdServerDataSource.CommitType.Insert);

            GdServerDataSource dataSource = CreateNewDataSource;
            List<GdMemoryTable> memoryTables = dataSource.Commit(tableSet);
            
            Assert.AreEqual(memoryTables.Count, 2);
        }
    }
}