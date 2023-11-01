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

        private void button2_Click(object sender, EventArgs e)
        {
            double x1 = Convert.ToDouble(minXTextBox.Text, CultureInfo.InvariantCulture);
            double y1 = Convert.ToDouble(minYTextBox.Text, CultureInfo.InvariantCulture);
            double x2 = Convert.ToDouble(maxXTextBox.Text, CultureInfo.InvariantCulture);
            double y2 = Convert.ToDouble(maxYTextBox.Text, CultureInfo.InvariantCulture);

            int min = Convert.ToInt32(minZoomLevelText.Text, CultureInfo.InvariantCulture);
            int max = Convert.ToInt32(maxZoomLevelText.Text, CultureInfo.InvariantCulture);

            string routePath = _defPath;
            if (!string.IsNullOrWhiteSpace(outPutTextBox.Text))
                routePath = outPutTextBox.Text;

            GdFileLogger.Current.InitializeLogger(Path.Combine(routePath, "tilelog"));
            
            GdGdalDataSource dataSource = GdGdalDataSource.Open(fileTextBox.Text);

            const string name = "BingMaps";
            const string format = "jpg";
            GlobalSphericalMercator schema = new GlobalSphericalMercator(format, YAxis.OSM, 1, 21, name);
            Extent extent = new Extent(x1, y1, x2, y2);

            for (int i = min; i <= max; i++)
            {
                //log
                IEnumerable<TileInfo> tileInfos = schema.GetTileInfos(extent, i);
                int len = tileInfos.Count();

                string message = $"level {i} ---- {len} tile started";
                GdFileLogger.Current.Log(message, LogType.Info);

                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();

                long count = 0;
                foreach (TileInfo index in tileInfos)
                {
                    WriteHeight(dataSource, index, routePath);
                    count++;
                }

                TimeSpan ts = stopWatch.Elapsed;
                stopWatch.Stop();

                //log
                message = $"level {i} --- {count} file --- {ts.Days} day {ts.Hours} hour {ts.Minutes} minutes {ts.Seconds} second";
                GdFileLogger.Current.Log(message, LogType.Info);
            }

            MessageBox.Show("Bitti!:  ");
        }

        private void WriteHeight(GdGdalDataSource ds, TileInfo index, string path)
        {
            double arrayLenght = 65;
            int densityVal = 13; //0, 5, 13, 65 olabilir....65 katları...
            
            //create folder
            string zPath = Path.Combine(path, index.Index.Level.ToString());
            if (!Directory.Exists(zPath))
                Directory.CreateDirectory(zPath);

            string xPath = Path.Combine(zPath, index.Index.Col.ToString());
            if (!Directory.Exists(xPath))
                Directory.CreateDirectory(xPath);

            Envelope envelope = new Envelope(index.Extent.MinX, index.Extent.MaxX, index.Extent.MinY, index.Extent.MaxY);

            //create envelope file(debug için kullan)
            //Geometry geometry = GdFactoryFinder.Instance.GeometryServices.CreateGeometryFactory().ToGeometry(envelope);
            //File.WriteAllText(Path.Combine(xPath, index.Index.Row + ".json"), ToJson(geometry));

            double stepX = envelope.Width / arrayLenght;
            double stepY = envelope.Height / arrayLenght;

            Heights heights = new Heights();
            
            int rowCount = 0;
            for (double j = envelope.MaxY; j >= envelope.MaxY - envelope.Height; j -= stepY)
            {
                ////bir üst satırı kopyala
                if (rowCount % densityVal != 0)
                {
                    Row copy = heights[rowCount - 1].Copy(j);
                    heights.Add(copy);
                    rowCount++;
                    continue;
                }

                //read
                int colCount = 0;
                double height = -1000;
                Row row = new Row();
                for (double i = envelope.MinX; i <= envelope.MinX + envelope.Width; i += stepX)
                {
                    if (colCount % densityVal == 0) //0, 5, 13, 65 olabilir....65 katları...
                    {
                        Coordinate unProject = ds.UnProject(i, j); //pixel
                        double[] pixelVals = ds.ReadBand(1, (int)unProject.X, (int)unProject.Y, new Size(1, 1));
                        height = pixelVals[0];
                    }
                    RowPoint rowPoint = new RowPoint(new Coordinate(i, j), height);
                    row.Add(rowPoint);

                    colCount++;
                    
                    if (colCount == arrayLenght)//double artık değerlerden dolayı
                        break;
                }

                if (rowCount == arrayLenght)//double artık değerlerden dolayı
                    break;
                
                rowCount++;
                heights.Add(row);
            }

            //sqllite(debug için kullan)
            //WriteSQlite(heights, Path.Combine(xPath, index.Index.Row + ".sqlite"));

            //create buffer text
            File.WriteAllText(Path.Combine(xPath, index.Index.Row + ".txt"), heights.ToString());
        }

        private void WriteSQlite(List<Row> heights, string fileName)
        {
            //////create sqlite....
            GdSqlLiteDataSource sqlLiteDataSource = GdSqlLiteDataSource.OpenOrCreate(fileName);
            GdSqlLiteTable liteTable = sqlLiteDataSource.CreateTable("height", GdGeometryType.Point, 3857, null);
            liteTable.CreateField(new GdField("x", GdDataType.Real));
            liteTable.CreateField(new GdField("y", GdDataType.Real));
            liteTable.CreateField(new GdField("height", GdDataType.Real));

            sqlLiteDataSource.BeginTransaction();

            foreach (Row rowList in heights)
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

        private class Heights : List<Row>
        {
            public override string ToString()
            {
                string join = string.Join(",", this);
                return join;
            }
        }

        private class Row : List<RowPoint>
        {
            public Row Copy(double y)
            {
                Row result = new Row();
                foreach (RowPoint point in this)
                {
                    Coordinate coordinate = new Coordinate(point.Coordinate.X, y);
                    RowPoint pnt = new RowPoint(coordinate, point.Height);
                    result.Add(pnt);
                }
                return result;
            }

            public override string ToString()
            {
                string join = string.Join(",", this);
                return join;
            }
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