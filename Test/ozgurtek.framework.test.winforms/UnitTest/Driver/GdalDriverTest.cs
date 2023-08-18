using NUnit.Framework;
using ozgurtek.framework.driver.gdal;
using System.Collections.Generic;
using System.Drawing;
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

        [Test]
        public void GeoTransformTest()
        {
            GdGdalDataSource dataSource = GdGdalDataSource.Open(_path);
            Coordinate unProject = dataSource.UnProject(579075.6, 4187474.6);
            double readPixel = dataSource.ReadBand(1, (int)unProject.X, (int)unProject.Y);
        }

        [Test]
        public void ExportTest()
        {
            GdGdalDataSource dataSource = GdGdalDataSource.Open(_path);
            int w = dataSource.RasterWidth;
            int h = dataSource.RasterHeight;

            List<string> result = new List<string>();
            Coordinate unProject = dataSource.UnProject(579075.6, 4187474.6);
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    double pixelVal = dataSource.ReadBand(1, i, j);
                    Coordinate coordinate = dataSource.Project(i, j);
                    result.Add($"{coordinate.X}-{coordinate.Y}-{pixelVal}" );
                }
            }

            string xtx = string.Join(" ", result);
        }

    }
}
