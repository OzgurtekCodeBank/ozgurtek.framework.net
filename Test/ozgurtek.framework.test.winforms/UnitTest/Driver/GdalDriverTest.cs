using NUnit.Framework;
using ozgurtek.framework.driver.gdal;
using System.Collections.Generic;
using System.Drawing;
using NetTopologySuite.Geometries;
using OSGeo.GDAL;
using ozgurtek.framework.common.Data;
using ozgurtek.framework.common.Geodesy;
using ozgurtek.framework.core.Data;
using ozgurtek.framework.driver.sqlite;
using Point = NetTopologySuite.Geometries.Point;

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
            GdSqlLiteDataSource sqlLiteDataSource = GdSqlLiteDataSource.OpenOrCreate("C:\\Users\\eniso\\Desktop\\height.sqlite");
            GdSqlLiteTable liteTable = sqlLiteDataSource.CreateTable("height", GdGeometryType.Point, 4326, null);
            liteTable.CreateField(new GdField("x", GdDataType.Real));
            liteTable.CreateField(new GdField("y", GdDataType.Real));
            liteTable.CreateField(new GdField("height", GdDataType.Real));
            liteTable.CreateField(new GdField("str", GdDataType.Real));

            GdGdalDataSource dataSource = GdGdalDataSource.Open(_path);
            int w = dataSource.RasterWidth;
            int h = dataSource.RasterHeight;

            int density = 100;
            sqlLiteDataSource.BeginTransaction();
            for (int i = 0; i < w; i += density)
            {
                for (int j = 0; j < h; j += density)
                {
                    double pixelVal = dataSource.ReadBand(1, i, j);
                    Coordinate coordinate = dataSource.Project(i, j);
                    Coordinate project = GdProjection.Project(coordinate, 5253, 4326);
                    Point point = new Point(project);
                    point.SRID = 4326;

                    GdRowBuffer buffer = new GdRowBuffer();
                    buffer.Put("geometry", point);
                    buffer.Put("x", project.X);
                    buffer.Put("y", project.Y);
                    buffer.Put("height", pixelVal);
                    buffer.Put("str", $"{coordinate.X}-{coordinate.Y}-{pixelVal}");
                    liteTable.Insert(buffer);
                }
            }
            sqlLiteDataSource.CommitTransaction();
        }

    }
}
