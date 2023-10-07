using System;
using NUnit.Framework;
using ozgurtek.framework.driver.gdal;
using System.Collections.Generic;
using System.Drawing;
using NetTopologySuite.Geometries;
using OSGeo.GDAL;
using ozgurtek.framework.common.Data;
using ozgurtek.framework.common.Data.Format.OnlineMap.Google;
using ozgurtek.framework.common.Geodesy;
using ozgurtek.framework.core.Data;
using ozgurtek.framework.driver.sqlite;
using Point = NetTopologySuite.Geometries.Point;
using ozgurtek.framework.common.Util;

namespace ozgurtek.framework.test.winforms.UnitTest.Driver
{
    [TestFixture]
    public class GdalDriverTest
    {
        private readonly string _inputPath1 = @"C:\data\work\unittest\unit_test_1.tif";
        private readonly string _inputPath2 = @"C:\data\work\unittest\unit_test_2.ecw";
        private readonly string _inputPath3 = @"C:\data\work\unittest\TURKEY_BIL\turkey.bil";
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
            dataSource.Dispose();
        }

        [Test]
        public void EnvelopeTest()
        {
            GdGdalDataSource dataSource = GdGdalDataSource.Open(_inputPath1);
            Envelope envelope = dataSource.Envelope;
            Assert.IsNotNull(envelope);
            dataSource.Dispose();
        }

        [Test]
        public void ProjectionTest()
        {
            GdGdalDataSource dataSource = GdGdalDataSource.Open(_inputPath1);
            string projectionString = dataSource.ProjectionString;
            Assert.IsNotNull(projectionString);
            dataSource.Dispose();
        }

        [Test]
        public void ReadRaster()
        {
            GdGdalDataSource dataSource = GdGdalDataSource.Open(_inputPath2);
            Size size = new Size(200, 200);
            Rectangle source = new Rectangle(dataSource.RasterWidth / 2, dataSource.RasterHeight / 2, 200, 200);
            Bitmap bitmap = dataSource.ReadRaster(source, size);
            bitmap.Save(_outputPath + Guid.NewGuid() + ".png");
            dataSource.Dispose();
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
            dataSource.Dispose();
        }

        [Test]
        public void ReadRaster3()
        {
            GdTileIndex index = new GdTileIndex(293, 196, 9);
            Size requestSize = new Size(65, 65);

            //tile'ın geometrisini buluyoruz...
            GdGoogleMapsTileMatrixSet set = new GdGoogleMapsTileMatrixSet();
            GdTileMatrixCalculator cal = new GdTileMatrixCalculator(set);
            Polygon geom3857 = cal.GetGeometry(index);
            geom3857.SRID = 3857;
            Geometry geom4326 = GdProjection.Project(geom3857, 4326);
            string text = geom4326.ToText();

            GdGdalDataSource dataSource = GdGdalDataSource.Open(_inputPath3);
            Envelope envelopeInternal = geom4326.EnvelopeInternal;
            Coordinate llPx = dataSource.UnProject(envelopeInternal.MinX, envelopeInternal.MinY);
            Coordinate urPx = dataSource.UnProject(envelopeInternal.MaxX, envelopeInternal.MaxY);
            int dx = (int)Math.Abs(llPx.X - urPx.X);
            int dy = (int)Math.Abs(llPx.Y - urPx.Y);
            Size pxSize = new Size(dx, dy);

            double[] readBand = dataSource.ReadBand(1, (int)llPx.X, (int)urPx.Y, pxSize, requestSize);
            dataSource.Dispose();
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
                    double[] pixelVals = dataSource.ReadBand(1, i, j, new Size(1, 1));
                    if (pixelVals.Length == 0)
                        continue;

                    double pixelVal = pixelVals[0];
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
