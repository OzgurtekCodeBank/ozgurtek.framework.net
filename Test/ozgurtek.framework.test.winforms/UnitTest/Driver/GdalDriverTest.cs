using System;
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
        private readonly string _inputPath1 = @"C:\data\work\unittest\unit_test_1.tif";
        private readonly string _inputPath2 = @"C:\data\work\unittest\unit_test_2.ecw";
        private readonly string _outputPath = @"C:\\Users\\eniso\\Desktop\\";

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
            GdGdalDataSource dataSource = GdGdalDataSource.Open(_inputPath1);
            Dataset dataset = dataSource.GdalDataSource;
            Assert.IsNotNull(dataset);
        }

        [Test]
        public void EnvelopeTest()
        {
            GdGdalDataSource dataSource = GdGdalDataSource.Open(_inputPath1);
            Envelope envelope = dataSource.Envelope;
            Assert.IsNotNull(envelope);
        }

        [Test]
        public void ProjectionTest()
        {
            GdGdalDataSource dataSource = GdGdalDataSource.Open(_inputPath1);
            string projectionString = dataSource.ProjectionString;
            Assert.IsNotNull(projectionString);
        }

        [Test]
        public void ReadRaster()
        {
            GdGdalDataSource dataSource = GdGdalDataSource.Open(_inputPath2);
            Size size = new Size(200, 200);
            Rectangle source = new Rectangle(dataSource.RasterWidth / 2, dataSource.RasterHeight / 2, 200, 200);
            Bitmap bitmap = dataSource.ReadRaster(source, size);
            bitmap.Save(_outputPath + Guid.NewGuid() + ".png");
        }

        [Test]
        public void ReadRaster2()
        {
            GdGdalDataSource dataSource = GdGdalDataSource.Open(_inputPath2);
            Size size = new Size(200, 200);
            Coordinate p1 = new Coordinate(28.26752, 39.28045);
            Coordinate p2 = new Coordinate(28.27903, 39.28789);
            Envelope envelope = new Envelope(p1, p2);
            Bitmap bitmap = dataSource.ReadRaster(envelope, size);
            bitmap.Save(_outputPath + Guid.NewGuid() + ".png");
        }

        [Test]
        public void ExportPixelTest()
        {
            GdSqlLiteDataSource sqlLiteDataSource = GdSqlLiteDataSource.OpenOrCreate(_outputPath + Guid.NewGuid() + ".sqlite");
            GdSqlLiteTable liteTable = sqlLiteDataSource.CreateTable("height", GdGeometryType.Point, 4326, null);
            liteTable.CreateField(new GdField("x", GdDataType.Real));
            liteTable.CreateField(new GdField("y", GdDataType.Real));
            liteTable.CreateField(new GdField("height", GdDataType.Real));

            GdGdalDataSource dataSource = GdGdalDataSource.Open(_inputPath1);
            int w = dataSource.RasterWidth;
            int h = dataSource.RasterHeight;

            int density = 100;
            sqlLiteDataSource.BeginTransaction();
            for (int i = 0; i < w; i += density)
            {
                for (int j = 0; j < h; j += density)
                {
                    double pixelVal = dataSource.ReadBand(1, i, j);
                    if (pixelVal < 0)
                        continue;

                    Coordinate coordinate = dataSource.Project(i, j);
                    Coordinate project = GdProjection.Project(coordinate, 5253, 4326);
                    Point point = new Point(project);
                    point.SRID = 4326;

                    GdRowBuffer buffer = new GdRowBuffer();
                    buffer.Put("geometry", point);
                    buffer.Put("x", project.X);
                    buffer.Put("y", project.Y);
                    buffer.Put("height", pixelVal);
                    liteTable.Insert(buffer);
                }
            }
            sqlLiteDataSource.CommitTransaction();
            dataSource.Dispose();
        }
    }
}
