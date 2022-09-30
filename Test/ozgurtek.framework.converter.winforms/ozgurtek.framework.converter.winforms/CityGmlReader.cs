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
            list.Add(memTable);

            XmlReader xmlReader = XmlReader.Create(filePath);
            while (xmlReader.Read())
            {
                if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "gml:LinearRing")
                {
                    GdRowBuffer buffer = new GdRowBuffer();
                    string attribute = xmlReader.GetAttribute("gml:id");
                    buffer.Put(Util.GdId, attribute);

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

                            buffer.Put(Util.GdGeometry, polygon);
                            memTable.Insert(buffer);
                        }
                    }
                }
            }

            return list;
        }
    }
}