using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Xml;
using NetTopologySuite.Geometries;
using ozgurtek.framework.common.Data;
using ozgurtek.framework.common.Data.Format;
using ozgurtek.framework.common.Style;
using ozgurtek.framework.core.Data;

namespace ozgurtek.framework.converter.winforms
{
    public class CityGmlReader
    {
        public IEnumerable<IGdTable> Read(string filePath)
        {
            List<GdMemoryTable> list = new List<GdMemoryTable>();

            GdMemoryTable memTable = Util.PrepareNewMemTable();
            memTable.Name = "Building";
            Read(filePath, memTable);

            list.Add(memTable);

            return list;
        }

        private void Read(string filePath, GdMemoryTable memoryTable)
        {
            XmlReader xmlReader = XmlReader.Create(filePath);
            while (xmlReader.Read())
            {
                if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "gml:LinearRing") //each triangle
                {
                    GdRowBuffer buffer = new GdRowBuffer();
                    
                    //id
                    string id = xmlReader.GetAttribute("gml:id");
                    buffer.Put(Util.GdId, id);

                    //geometry
                    Geometry geometry = GetGeometry(xmlReader);
                    buffer.Put(Util.GdGeometry, geometry);
                    
                    //material
                    byte[] material = GetMaterial(filePath, id);
                    GdPolygonStyle polygonStyle = new GdPolygonStyle();
                    polygonStyle.Stroke = null;
                    GdFill fill = new GdFill();
                    fill.Material = material;
                    polygonStyle.Fill = fill;
                    //GdStyleJsonSerializer serializer = new GdStyleJsonSerializer();
                    //string serialize = serializer.Serialize(polygonStyle);

                    //buffer.Put("gd_style", serialize);

                    memoryTable.Insert(buffer);
                }
            }
        }

        private Geometry GetGeometry(XmlReader xmlReader)
        {
            while (xmlReader.Read())
            {
                if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "gml:posList")
                {
                    string coords = xmlReader.ReadInnerXml();
                    string[] strings = coords.Split(' ');

                    List<Coordinate> coordinates = new List<Coordinate>();
                    for (int i = 0; i < strings.Length - 1; i += 3)
                    {
                        CoordinateZ coordinate = new CoordinateZ(
                            double.Parse(strings[i], CultureInfo.InvariantCulture),
                            double.Parse(strings[i + 1], CultureInfo.InvariantCulture));

                        coordinate.Z = double.Parse(strings[i + 2], CultureInfo.InvariantCulture);
                        coordinates.Add(coordinate);
                    }

                    LinearRing ring = new LinearRing(coordinates.ToArray());
                    Polygon polygon = new Polygon(ring);
                    return polygon;
                }
            }

            return null;
        }


        private byte[] GetMaterial(string filePath, string tag)
        {
            XmlReader xmlReader = XmlReader.Create(filePath);

            while (xmlReader.Read())
            {
                if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "app:ParameterizedTexture")
                {
                    while (xmlReader.Read())
                    {
                        if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "app:imageURI")
                        {
                            string imageFile = xmlReader.ReadInnerXml();
                            while (xmlReader.Read())
                            {
                                if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "app:textureCoordinates")
                                {
                                    string id = xmlReader.GetAttribute("ring");
                                    if (tag.Equals(id))
                                    {
                                        string coordinates = xmlReader.ReadInnerXml();
                                        return Crop(id, imageFile, coordinates);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return null;
        }

        private byte[] Crop(string id, string imageFile, string textureCoordinate)
        {
            string[] textureCoordArray = textureCoordinate.Split(' ');

            PointF[] pointArray = new PointF[4];
            Bitmap originalTexturebm = (Bitmap)Image.FromFile(imageFile);
            for (int i = 0; i < textureCoordArray.Length / 2; i++)
            {
                pointArray[i].X = float.Parse(textureCoordArray[i * 2], CultureInfo.InvariantCulture.NumberFormat) * originalTexturebm.Width;
                pointArray[i].Y = (1 - float.Parse(textureCoordArray[i * 2 + 1], CultureInfo.InvariantCulture.NumberFormat)) * originalTexturebm.Height;
            }

            int seperatedTextureWidth = (int)(pointArray.Max(p => p.X) - pointArray.Min(p => p.X));
            int seperatedTextureHeight = (int)(pointArray.Max(p => p.Y) - pointArray.Min(p => p.Y));
            Bitmap seperatedTexturebm = new Bitmap(seperatedTextureWidth, seperatedTextureHeight);

            GraphicsPath gp = new GraphicsPath();
            gp.AddPolygon(pointArray);
            using (Graphics G = Graphics.FromImage(seperatedTexturebm))
            {
                G.Clip = new Region(gp);
                G.DrawImage(originalTexturebm, 0, 0);
                seperatedTexturebm.(@"d:\3_8_0_test.jpg");
            }

            gp.Dispose();
        }
    }
}