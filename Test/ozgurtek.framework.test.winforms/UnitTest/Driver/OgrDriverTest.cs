using System.Collections.Generic;
using NUnit.Framework;
using ozgurtek.framework.core.Data;
using ozgurtek.framework.driver.gdal;

namespace ozgurtek.framework.test.winforms.UnitTest.Driver
{
    [TestFixture]
    public class OgrDriverTest
    {
        //private string _source = @"C:\Users\eniso\Desktop\work\testdata\shp\mahalle.shp";
        private string _source = @"WFS:http://185.122.200.110:8080/geoserver/dhmi/wfs?service=WFS";

        [Test]
        public void DriverTest()
        {
            IEnumerable<string> driverNames = GdOgrDataSource.DriverNames;
            List<string> driverName = new List<string>(driverNames);
            Assert.GreaterOrEqual(driverName.Count, 1);
        }

        /**
         * Veritabanındaki tablo sayısını öğrenmek
         */
        [Test]
        public void TableCountTest()
        {
            GdOgrDataSource dataSource = GdOgrDataSource.Open(_source);
            Assert.GreaterOrEqual(dataSource.TableCount, 1);
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
            IGdTable table = GetTable("MAHALLE");
            Assert.IsNotNull(table);
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
