using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BruTile;
using BruTile.Predefined;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using Newtonsoft.Json;
using ozgurtek.framework.common;
using ozgurtek.framework.common.Data;
using ozgurtek.framework.common.Data.Format.OnlineMap.Google;
using ozgurtek.framework.common.Geodesy;
using ozgurtek.framework.common.Util;
using ozgurtek.framework.core.Data;
using ozgurtek.framework.driver.gdal;
using ozgurtek.framework.driver.sqlite;
using Envelope = NetTopologySuite.Geometries.Envelope;
using Point = NetTopologySuite.Geometries.Point;

namespace ozgurtek.framework.test.winforms
{
    public partial class TestForm : Form
    {
        private string _source = @"C:\Users\eniso\Desktop\work\testdata\shp\mahalle.shp";
        private string _wfsSource = @"WFS:http://185.122.200.110:8080/geoserver/dhmi/wfs?service=WFS";
        private string _defPath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\Export\";

        public TestForm()
        {
            InitializeComponent();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Coordinate cor1 = new Coordinate(27.8778076171875, 37.815411006369104);
            Coordinate cor2 = new Coordinate(27.88330078125, 37.820904170431604);
            var coordinate = GdProjection.Project(cor1, 4326, 5253);
            var coordinate2 = GdProjection.Project(cor2, 4326, 5253);

            string source = "C:\\TURKEY_DTED\\turkey_DTED.tif";
            GdGdalDataSource dataSource = GdGdalDataSource.Open(source);
            // WriteHeight(dataSource, new GdTileIndex(39049, 25104, 16));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            double x1 = Convert.ToDouble(minXTextBox.Text, CultureInfo.InvariantCulture);
            double y1 = Convert.ToDouble(minYTextBox.Text, CultureInfo.InvariantCulture);
            double x2 = Convert.ToDouble(maxXTextBox.Text, CultureInfo.InvariantCulture);
            double y2 = Convert.ToDouble(maxYTextBox.Text, CultureInfo.InvariantCulture);

            int min = Convert.ToInt32(minZoomLevelText.Text, CultureInfo.InvariantCulture);
            int max = Convert.ToInt32(maxZoomLevelText.Text, CultureInfo.InvariantCulture);

            GdGdalDataSource dataSource = GdGdalDataSource.Open(fileTextBox.Text);

            const string name = "BingMaps";
            const string format = "jpg";
            GlobalSphericalMercator schema = new GlobalSphericalMercator(format, YAxis.OSM, 1, 21, name);
            Extent extent = new Extent(x1, y1, x2, y2);
            double rasterDensity = CalcDesity();

            for (int i = min; i <= max; i++)
            {
                IEnumerable<TileInfo> tileInfos = schema.GetTileInfos(extent, i);
                foreach (TileInfo index in tileInfos)
                {
                    Stopwatch stopWatch = new Stopwatch();
                    stopWatch.Start();

                    WriteHeight(dataSource, index, rasterDensity, 65);
                    
                    stopWatch.Stop();
                    TimeSpan ts = stopWatch.Elapsed;
                    string elapsedTime = ts.Milliseconds.ToString();
                }
            }

            MessageBox.Show("Bitti!");
        }

        private void WriteHeight(GdGdalDataSource ds, TileInfo index, double density, double arrayLenght)
        {
            string path = _defPath;

            //create folder
            string zPath = Path.Combine(path, index.Index.Level.ToString());
            if (!Directory.Exists(zPath))
                Directory.CreateDirectory(zPath);

            string xPath = Path.Combine(zPath, index.Index.Col.ToString());
            if (!Directory.Exists(xPath))
                Directory.CreateDirectory(xPath);

            Envelope envelope = new Envelope(index.Extent.MinX, index.Extent.MaxX, index.Extent.MinY, index.Extent.MaxY);

            //create envelope file
            Geometry geometry = GdFactoryFinder.Instance.GeometryServices.CreateGeometryFactory().ToGeometry(envelope);
            File.WriteAllText(Path.Combine(xPath, index.Index.Row + ".json"), ToJson(geometry));

            double stepX = envelope.Width / arrayLenght;
            double stepY = envelope.Height / arrayLenght;

            int densityVal = 5;
            List<RowList> heights = new List<RowList>();
            
            int rowCount = 0;
            for (double j = envelope.MaxY; j >= envelope.MaxY - envelope.Height; j -= stepY)
            {
                ////bir üst satırı kopyala
                //if (rowCount % densityVal != 0)
                //{
                //    RowList list = heights[rowCount - 1];
                //    heights.Add(list);
                //    rowCount++;
                //    continue;
                //}

                //read
                int colCount = 0;
                double height = -1000;
                RowList rowList = new RowList();
                for (double i = envelope.MinX; i <= envelope.MinX + envelope.Width; i += stepX)
                {
                    Coordinate unProject = ds.UnProject(i, j); //pixel
                    double[] pixelVals = ds.ReadBand(1, (int)unProject.X, (int)unProject.Y, new Size(1, 1));
                    height = pixelVals[0];
                    RowPoint rowPoint = new RowPoint(new Coordinate(i, j), height);
                    rowList.Add(rowPoint);

                    if (colCount % densityVal == 0) //gerçekten oku
                    {
                        
                    }
                    colCount++;
                }

                heights.Add(rowList);
                rowCount++;
            }

            //sqllite
            WriteSQlite(heights, Path.Combine(xPath, index.Index.Row + ".sqlite"));

            //create buffer text
            //File.WriteAllText(Path.Combine(xPath, index.Index.Row + ".txt"), string.Join(",", result));
        }

        private void WriteSQlite(List<RowList> heights, string fileName)
        {
            //////create sqlite....
            GdSqlLiteDataSource sqlLiteDataSource = GdSqlLiteDataSource.OpenOrCreate(fileName);
            GdSqlLiteTable liteTable = sqlLiteDataSource.CreateTable("height", GdGeometryType.Point, 3857, null);
            liteTable.CreateField(new GdField("x", GdDataType.Real));
            liteTable.CreateField(new GdField("y", GdDataType.Real));
            liteTable.CreateField(new GdField("height", GdDataType.Real));

            sqlLiteDataSource.BeginTransaction();

            foreach (RowList rowList in heights)
            { 
                foreach (RowPoint rowPoint in rowList)
                {
                    Point point = GdFactoryFinder.Instance.GeometryServices.CreateGeometryFactory().CreatePoint(rowPoint.Coordinate);
                    point.SRID = 3857;
                    GdRowBuffer buffer = new GdRowBuffer();
                    buffer.Put("geometry", point);
                    buffer.Put("x", rowPoint.Coordinate.X);
                    buffer.Put("y", rowPoint.Coordinate.Y);
                    buffer.Put("height", rowPoint.Height);
                    liteTable.Insert(buffer);
                }
            }

            sqlLiteDataSource.CommitTransaction();
        }

        private class RowList : List<RowPoint>
        {
        }

        private class RowPoint
        {
            private readonly Coordinate _coordinate;
            private readonly double _height = -1000.0;

            public RowPoint(Coordinate coordinate, double height)
            {
                _coordinate = coordinate;
                _height = height;
            }

            public double Height
            {
                get { return _height; }
            }

            public Coordinate Coordinate
            {
                get { return _coordinate; }
            }

            public override string ToString()
            {
                return _height.ToString(CultureInfo.InvariantCulture);
            }
        }

        public string ToJson(Geometry geometry)
        {
            StringBuilder sb = new StringBuilder();
            StringWriter writer = new StringWriter(sb);
            GeometryFactory geometryFactory = GdFactoryFinder.Instance.GeometryServices.CreateGeometryFactory();
            JsonSerializer jsonSerializer = GeoJsonSerializer.Create(geometryFactory, 2);
            jsonSerializer.Serialize(writer, geometry);
            writer.Flush();
            return writer.ToString();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string source = "C:\\aydin3857.tif";
            GdGdalDataSource dataSource = GdGdalDataSource.Open(source);
            //WriteHeight(dataSource, new GdTileIndex(75689, 50642, 17));
        }

        private void densityCalcButton_Click(object sender, EventArgs e)
        {
            try
            {
                double calcDesity = CalcDesity();
                densityResultLabel.Text = calcDesity.ToString(CultureInfo.InvariantCulture) + " m";
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void calcTileButton_Click(object sender, EventArgs e)
        {
            double x1 = Convert.ToDouble(minXTextBox.Text, CultureInfo.InvariantCulture);
            double y1 = Convert.ToDouble(minYTextBox.Text, CultureInfo.InvariantCulture);
            double x2 = Convert.ToDouble(maxXTextBox.Text, CultureInfo.InvariantCulture);
            double y2 = Convert.ToDouble(maxYTextBox.Text, CultureInfo.InvariantCulture);
            Coordinate cor1 = new Coordinate(x1, y1);
            Coordinate cor2 = new Coordinate(x2, y2);

            int min = Convert.ToInt32(minZoomLevelText.Text, CultureInfo.InvariantCulture);
            int max = Convert.ToInt32(maxZoomLevelText.Text, CultureInfo.InvariantCulture);

            GdGoogleMapsTileMatrixSet matrixSet = new GdGoogleMapsTileMatrixSet();
            GdTileMatrixCalculator calculator = new GdTileMatrixCalculator(matrixSet);

            Envelope filerEnvelope = new Envelope(cor1, cor2);
            filerEnvelope = GdProjection.Project(filerEnvelope, 4326, 3857);

            long size = 0;
            for (int i = min; i <= max; i++)
            {
                size += calculator.CalAreaTileCount(filerEnvelope, i, 0);
            }

            tileSizelabel.Text = size.ToString();
        }

        private double CalcDesity()
        {
            GdGdalDataSource dataSource = GdGdalDataSource.Open(fileTextBox.Text);
            double w = dataSource.Envelope.Width / dataSource.RasterWidth;
            double h = dataSource.Envelope.Height / dataSource.RasterHeight;
            return w;
        }
    }
}