using System;
using System.Collections.Generic;
using System.IO;
using NetTopologySuite.Geometries;
using NUnit.Framework;
using ozgurtek.framework.core.Data;
using ozgurtek.framework.driver.gdal;

namespace ozgurtek.framework.test.winforms.UnitTest.Driver
{
    [TestFixture]
    public class OgrDriverTest
    {
        private string _path = @"C:\Users\eniso\Desktop\work\testdata\shp\";

        private string _source = @"C:\Users\eniso\Desktop\work\testdata\shp\mahalle.shp";
        //private string _source = @"WFS:http://185.122.200.110:8080/geoserver/dhmi/wfs?service=WFS";
        //private string _source = @"C:\Users\eniso\Desktop\work\testdata\test.gml";
        //private string _source = @"C:\Users\eniso\Desktop\work\testdata\test.kmz";

        [Test]
        public void DriverTest()
        {
            IEnumerable<string> driverNames = GdOgrDataSource.DriverNames;
            List<string> driverName = new List<string>(driverNames);
            Assert.GreaterOrEqual(driverName.Count, 1);
        }

        [Test]
        public void CreateDataSourceTest()
        {
            string driverName = "ESRI Shapefile";
            string file = Path.Combine(_path, Guid.NewGuid() + ".shp");
            GdOgrDataSource dataSource = GdOgrDataSource.Create(driverName, file, null);
            GdOgrTable ogrTable = dataSource.CreateTable("layer1", GdGeometryType.Polygon, 4326, null);
            Assert.NotNull(ogrTable);
        }

        [Test]
        public void TableCountTest()
        {
            GdOgrDataSource dataSource = GdOgrDataSource.Open(_source);
            Assert.GreaterOrEqual(dataSource.TableCount, 1);
        }

        [Test]
        public void GetTablesTest()
        {
            IEnumerable<IGdTable> tables = GetTable();
            List<IGdTable> clone = new List<IGdTable>(tables);
            Assert.GreaterOrEqual(clone.Count, 1);
        }

        [Test]
        public void GetTableTest()
        {
            IEnumerable<IGdTable> tables = GetTable();
            List<IGdTable> clone = new List<IGdTable>(tables);
            IGdTable table = GetTable(clone[0].Name);
            Assert.IsNotNull(table);
        }

        [Test]
        public void GetRowCountTest()
        {
            IEnumerable<IGdTable> tables = GetTable();
            foreach (IGdTable table in tables)
            {
                int count = 0;
                foreach (IGdRow row in table.Rows)
                    count++;

                Assert.AreEqual(count, table.RowCount);
            }
        }

        [Test]
        public void GetRowTest()
        {
            IEnumerable<IGdTable> tables = GetTable();
            List<IGdRow> rows = new List<IGdRow>();
            foreach (IGdTable table in tables)
            {
                rows.AddRange(table.Rows);
            }

            Assert.GreaterOrEqual(rows.Count, 1);
        }

        [Test]
        public void GetGeomTest()
        {
            List<string> geomsString = new List<string>();

            IEnumerable<IGdTable> tables = GetTable();
            foreach (IGdTable table in tables)
            {
                foreach (IGdRow row in table.Rows)
                {
                    foreach (IGdParamater paramater in row.Paramaters)
                    {
                        if (row.IsNull(paramater.Name))
                            continue;

                        if (paramater.Value is Geometry geometry)
                        {
                            geomsString.Add(geometry.AsText());
                        }
                    }
                }
            }

            Assert.GreaterOrEqual(geomsString.Count, 1);
        }


        private IEnumerable<IGdTable> GetTable()
        {
            GdOgrDataSource dataSource = GdOgrDataSource.Open(_source);
            return dataSource.GetTable();
        }

        private IGdTable GetTable(string name)
        {
            GdOgrDataSource dataSource = GdOgrDataSource.Open(_source);
            return dataSource.GetTable(name);
        }
    }
}