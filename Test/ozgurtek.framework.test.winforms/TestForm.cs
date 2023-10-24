using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using JetBrains.dotMemoryUnit;
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
            //double x1 = Convert.ToDouble(textBox4.Text);
            //double y1 = Convert.ToDouble(textBox5.Text);
            //double x2 = Convert.ToDouble(textBox6.Text);
            //double y2 = Convert.ToDouble(textBox7.Text);
            //Coordinate cor1 = new Coordinate(x1, y1);
            //Coordinate cor2 = new Coordinate(x2, y2);

            int min = 20;/*Convert.ToInt32(textBox2.Text);*/
            int max = 20;/*Convert.ToInt32(textBox3.Text);*/
            
            string source = "C:\\TURKEY_DTED\\turkey_DTED.tif";
            GdGdalDataSource dataSource = GdGdalDataSource.Open(source);

            GdGoogleMapsTileMatrixSet matrixSet = new GdGoogleMapsTileMatrixSet();
            GdTileMatrixCalculator calculator = new GdTileMatrixCalculator(matrixSet);

            Coordinate cor1 = new Coordinate(34.4646353216893, 38.73258080727801);
            Coordinate cor2 = new Coordinate(34.62385274471592, 38.812034524942696);

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

        private void WriteHeight(GdGdalDataSource ds, GdTileIndex index, int sampleSize = 32)
        {
            string path = _defPath;
            if (!string.IsNullOrEmpty(textBox1.Text))
                path = textBox1.Text;

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
    }
}