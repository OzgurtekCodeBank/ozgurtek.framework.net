using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using Newtonsoft.Json;
using ozgurtek.framework.common;
using ozgurtek.framework.common.Data.Format.OnlineMap.Google;
using ozgurtek.framework.common.Geodesy;
using ozgurtek.framework.common.Util;
using ozgurtek.framework.core.Data;
using ozgurtek.framework.driver.gdal;
using Envelope = NetTopologySuite.Geometries.Envelope;

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
            WriteHeight(dataSource, new GdTileIndex(39049, 25104, 16));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            double x1 = Convert.ToDouble(minXTextBox.Text, CultureInfo.InvariantCulture);
            double y1 = Convert.ToDouble(minYTextBox.Text, CultureInfo.InvariantCulture);
            double x2 = Convert.ToDouble(maxXTextBox.Text, CultureInfo.InvariantCulture);
            double y2 = Convert.ToDouble(maxYTextBox.Text, CultureInfo.InvariantCulture);
            Coordinate cor1 = new Coordinate(x1, y1);
            Coordinate cor2 = new Coordinate(x2, y2);

            int min = Convert.ToInt32(minZoomLevelText.Text, CultureInfo.InvariantCulture);
            int max = Convert.ToInt32(maxZoomLevelText.Text, CultureInfo.InvariantCulture);

            GdGdalDataSource dataSource = GdGdalDataSource.Open(fileTextBox.Text);

            GdGoogleMapsTileMatrixSet matrixSet = new GdGoogleMapsTileMatrixSet();
            GdTileMatrixCalculator calculator = new GdTileMatrixCalculator(matrixSet);
            
            Envelope filerEnvelope = new Envelope(cor1, cor2);
            filerEnvelope = GdProjection.Project(filerEnvelope, 4326, 3857);

            for (int i = min; i <= max; i++)
            {
                IEnumerable<GdTileIndex> gdTileIndices = calculator.GetAreaTileList(filerEnvelope, i, 0, false);

                foreach (GdTileIndex index in gdTileIndices)
                {
                    WriteHeight(dataSource, index);
                }
            }

            MessageBox.Show("Bitti!");
        }

        private void WriteHeight(GdGdalDataSource ds, GdTileIndex index, int sampleSize = 65)
        {
            string path = _defPath;

            //create folder
            string zPath = Path.Combine(path, index.Z.ToString());
            if (!Directory.Exists(zPath))
                Directory.CreateDirectory(zPath);

            string xPath = Path.Combine(zPath, index.X.ToString());
            if (!Directory.Exists(xPath))
                Directory.CreateDirectory(xPath);

            GdGoogleMapsTileMatrixSet matrixSet = new GdGoogleMapsTileMatrixSet();
            GdTileMatrixCalculator calculator = new GdTileMatrixCalculator(matrixSet);
            Polygon geometry = calculator.GetGeometry(index);
            geometry.SRID = 3857;

            ////create envelope file
            //File.WriteAllText(Path.Combine(xPath, index.Y + ".json"), ToJson(geometry));

            //create sqlite....
            //GdSqlLiteDataSource sqlLiteDataSource = GdSqlLiteDataSource.OpenOrCreate(Path.Combine(xPath, index.Y + ".sqlite"));
            //GdSqlLiteTable liteTable = sqlLiteDataSource.CreateTable("height", GdGeometryType.Point, 3857, null);
            //liteTable.CreateField(new GdField("x", GdDataType.Real));
            //liteTable.CreateField(new GdField("y", GdDataType.Real));
            //liteTable.CreateField(new GdField("height", GdDataType.Real));

            Envelope envelope = geometry.EnvelopeInternal;
            double stepX = envelope.Width / sampleSize;
            double stepY = envelope.Height / sampleSize;

            List<double> heights = new List<double>();
            //sqlLiteDataSource.BeginTransaction();
            int ii = 0;
            for (double j = envelope.MaxY; j >= envelope.MaxY - envelope.Height; j -= stepY)
            {
                ii = 0;
                for (double i = envelope.MinX; i < envelope.MinX + envelope.Width; i += stepX)
                {
                    ii++;
                    if(ii>sampleSize)
                        break;

                    Coordinate unProject = ds.UnProject(i, j);//pixel
                    double[] pixelVals = ds.ReadBand(1, (int)unProject.X, (int)unProject.Y, new Size(1, 1));
                    double pixelVal = pixelVals[0];

                    //to txt
                    heights.Add(pixelVal);

                    //Point point = GdFactoryFinder.Instance.GeometryServices.CreateGeometryFactory().CreatePoint(new Coordinate(i, j));
                    //point.SRID = 3857;
                    //GdRowBuffer buffer = new GdRowBuffer();
                    //buffer.Put("geometry", point);
                    //buffer.Put("x", i);
                    //buffer.Put("y", j);
                    //buffer.Put("height", pixelVal);
                    //liteTable.Insert(buffer);

                    if (heights.Count >= sampleSize * sampleSize)
                        break;
                }

                if (heights.Count >= sampleSize * sampleSize)
                    break;
            }
            //sqlLiteDataSource.CommitTransaction();

            //create buffer text
            File.WriteAllText(Path.Combine(xPath, index.Y + ".txt"), string.Join(",", heights));

            ////write alternate type of height
            //Size requestSize = new Size(65, 65);
            //Coordinate ul = ds.UnProject(envelope.MinX, envelope.MaxY);
            //Coordinate lr = ds.UnProject(envelope.MaxX, envelope.MinY);
            //int dx = (int)Math.Abs(ul.X - lr.X);
            //int dy = (int)Math.Abs(ul.Y - lr.Y);
            //Size size = new Size(dx, dy);

            //double[] heights2 = ds.ReadBand(1, (int)ul.X, (int)ul.Y, size, requestSize);
            //File.WriteAllText(Path.Combine(xPath, index.Y + "_alt.txt"), string.Join(",", heights2));
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
            WriteHeight(dataSource, new GdTileIndex(75689, 50642, 17));
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