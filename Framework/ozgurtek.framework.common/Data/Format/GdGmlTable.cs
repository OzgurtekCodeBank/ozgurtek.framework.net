using System.IO;
using System.Xml;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO.GML2;
using ozgurtek.framework.core.Data;

namespace ozgurtek.framework.common.Data.Format
{
    public class GdGmlTable
    {
        public static GdMemoryTable LoadFromGml(string gml, string geometryFieldName = "geom")
        {
            return Load(gml, geometryFieldName);
        }

        private static GdMemoryTable Load(string gml, string geometryFieldName)
        {
            GdMemoryTable table = new GdMemoryTable();

            GMLReader geomGmlReader = new GMLReader();
            TextReader stringReader = new StringReader(gml);
            XmlDocument doc = new XmlDocument();
            doc.Load(stringReader);

            XmlElement root = doc.DocumentElement;
            if (root == null) 
                return table;
            
            XmlNodeList nodes = root.GetElementsByTagName("gml:featureMember");
            for (int i = 0; i < nodes.Count; i++)
            {
                XmlNode node = nodes[i];
                foreach (XmlNode childNode in node.ChildNodes)
                {
                    GdRowBuffer buffer = new GdRowBuffer();

                    //table name
                    SetBuffer(table, buffer, "table_name", childNode.Name);

                    //attributes
                    if (childNode.Attributes != null)
                    {
                        foreach (XmlAttribute attr in childNode.Attributes)
                        {
                            SetBuffer(table, buffer, attr.Name, attr.Value);
                        }
                    }

                    //nodes
                    foreach (XmlNode xmlNode in childNode.ChildNodes)
                    {
                        if (xmlNode.Name.Equals(geometryFieldName))
                        {
                            Geometry geometry = geomGmlReader.Read(xmlNode.InnerXml);
                            SetBuffer(table, buffer, xmlNode.Name, geometry);
                            continue;
                        }

                        SetBuffer(table, buffer, xmlNode.LocalName, xmlNode.InnerText);
                    }

                    table.Insert(buffer);
                }
            }

            return table;
        }

        private static void SetBuffer(GdMemoryTable table, GdRowBuffer buffer, string name, object value)
        {
            IGdField field = table.Schema.GetFieldByName(name);
            if (field == null)
            {
                table.CreateField(value is Geometry
                    ? new GdField(name, GdDataType.Geometry)
                    : new GdField(name, GdDataType.String));
            }

            if (value == null)
                buffer.PutNull(name);
            else
                buffer.Put(name, value);
        }
    }
}