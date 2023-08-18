using NUnit.Framework;
using ozgurtek.framework.driver.gdal;
using System.Collections.Generic;
using NetTopologySuite.Geometries;
using OSGeo.GDAL;
using ozgurtek.framework.common.Geodesy;

namespace ozgurtek.framework.test.winforms.UnitTest.Driver
{
    [TestFixture]
    public class GdalDriverTest
    {
        private string _path = @"C:\data\work\unittest\unit_test_1.tif";

        [Test]
        public void DriverTest()
        {
            IEnumerable<string> driverNames = GdGdalDataSource.DriverNames;
            List<string> driverName = new List<string>(driverNames);
            Assert.GreaterOrEqual(driverName.Count, 1);
        }

        [Test]
        public void OpenTest()
        {
            GdGdalDataSource dataSource = GdGdalDataSource.Open(_path);
            Dataset dataset = dataSource.GdalDataSource;
        }

        [Test]
        public void EnvelopeTest()
        {
            GdGdalDataSource dataSource = GdGdalDataSource.Open(_path);
            Envelope envelope = dataSource.Envelope;
        }

        [Test]
        public void ProjectionTest()
        {
            GdGdalDataSource dataSource = GdGdalDataSource.Open(_path);
            string projectionString = dataSource.ProjectionString;
        }


    }
}
