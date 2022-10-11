using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using NetTopologySuite.Geometries;
using ozgurtek.framework.common.Data;
using ozgurtek.framework.common.Data.Format;
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
                    Texture material = GetMaterial(filePath, id);
                    buffer.Put(Util.GdStyle, material.File);
                    buffer.Put(Util.GdTextureCoords, material.Coords);

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


        private Texture GetMaterial(string filePath, string tag)
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
                                        Texture texture = new Texture();
                                        texture.File = imageFile;
                                        texture.Coords = coordinates;
                                        return texture;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return null;
        }
    }

    public class Texture
    {
        public string Coords;
        public string File;
    }

}