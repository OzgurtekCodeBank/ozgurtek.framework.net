using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using ozgurtek.framework.common.Data.Format;
using ozgurtek.framework.core.Data;

namespace ozgurtek.framework.common.Data
{
    public class GdJsonTableDeserializer
    {
        public GdMemoryTable Deserialize(string json, bool useSchema = true)
        {
            return useSchema ? DeserializeWithSchema(json) : DeserializeWithoutSchema(json);
        }

        private GdMemoryTable DeserializeWithoutSchema(string json)
        {
            GdMemoryTable table = new GdMemoryTable();

            JObject data = JObject.Parse(json);
            if (!data.HasValues)
                return table;

            JToken jToken = data["features"];
            if (IsTokenNullOrEmpty(jToken))
                return table;

            JArray features = (JArray)jToken;
            foreach (JToken feature in features)
            {
                GdRowBuffer row = new GdRowBuffer();

                //put geometry field if exists
                JToken geometryToken = feature["geometry"];
                if (!IsTokenNullOrEmpty(geometryToken))
                {
                    table.CreateField(new GdField("geometry", GdDataType.Geometry));
                    object value = ParseValue(GdDataType.Geometry, geometryToken);
                    row.Put(table.GeometryField, value);
                }

                //put properties
                JObject token = (JObject)feature["properties"];
                if (token != null && token.HasValues)
                {
                    IEnumerable<JProperty> properties = token.Properties();
                    foreach (JProperty jProperty in properties)
                    {
                        string name = jProperty.Name;
                        if (string.IsNullOrWhiteSpace(name))
                            continue;

                        string value = string.Empty;
                        if (!jProperty.Value.HasValues)
                            value = jProperty.Value.ToString();
                        table.CreateField(new GdField(name, GdDataType.String));
                        row.Put(name, value);
                    }
                }
                table.Insert(row);
            }

            return table;
        }

        private GdMemoryTable DeserializeWithSchema(string json)
        {
            GdMemoryTable table = new GdMemoryTable();

            JObject data = JObject.Parse(json);
            JToken features = (JArray)data["features"];

            //create schema...
            CreateSchema(data, table);

            //create features...
            if (features != null)
            {
                foreach (JToken feature in features)
                {
                    GdRowBuffer row = new GdRowBuffer();

                    //put geometry field if exists
                    JToken geometryToken = feature["geometry"];
                    if (!IsTokenNullOrEmpty(geometryToken) && !string.IsNullOrEmpty(table.GeometryField))
                    {
                        object value = ParseValue(GdDataType.Geometry, geometryToken);
                        row.Put(table.GeometryField, value);
                    }

                    //put properties
                    JToken properties = feature["properties"];
                    foreach (IGdField field in table.Schema.Fields)
                    {
                        string fieldName = field.FieldName;
                        JToken properyToken = properties[fieldName];
                        if (IsTokenNullOrEmpty(properyToken))
                            continue;

                        object value = ParseValue(field.FieldType, properyToken);
                        row.Put(fieldName, value);
                    }

                    table.Insert(row);
                }
            }

            return table;
        }

        private void CreateSchema(JObject data, GdMemoryTable table)
        {
            JArray fields = (JArray)data["fields"];
            string keyfield = (string)data["keyfield"];
            string description = (string)data["description"];
            string geometryfield = (string)data["geometryfield"];
            string tableName = (string)data["name"];

            if (fields == null)
                throw new Exception("Schema needed...");

            foreach (JToken field in fields)
            {
                string name = (string)field["name"];
                if (name == geometryfield)
                    continue;

                string jtype = (string)field["type"];
                GdDataType type;
                switch (jtype)
                {
                    case "Integer":
                        type = GdDataType.Integer;
                        break;
                    case "Blob":
                        type = GdDataType.Blob;
                        break;
                    case "Date":
                        type = GdDataType.Date;
                        break;
                    case "Boolean":
                        type = GdDataType.Boolean;
                        break;
                    case "Geometry":
                        type = GdDataType.Geometry;
                        break;
                    case "Real":
                        type = GdDataType.Real;
                        break;
                    default:
                        type = GdDataType.String;
                        break;
                }
                table.CreateField(new GdField(name, type));
            }

            if (geometryfield != null)
            {
                table.CreateField(new GdField(geometryfield, GdDataType.Geometry));
                table.GeometryField = geometryfield;
            }

            if (keyfield != null)
            {
                table.KeyField = keyfield;
            }

            if (tableName != null)
            {
                table.Name = tableName;
            }

            if (description != null)
            {
                table.Description = description;
            }
        }

        private object ParseValue(GdDataType fieldType, JToken jToken)
        {
            switch (fieldType)
            {
                case GdDataType.Boolean:
                    return (bool)jToken;
                case GdDataType.Date:
                    return (DateTime)jToken;
                case GdDataType.Integer:
                    return (int)jToken;
                case GdDataType.Real:
                    return (double)jToken;
                case GdDataType.String:
                    return GetString(jToken);
                case GdDataType.Blob:
                    return DbConvert.ToBytes(GetString(jToken));
                case GdDataType.Geometry:
                    return DbConvert.ToGeometry(GetString(jToken));
                default:
                    return null;//impossible to reach
            }
        }

        private string GetString(JToken jToken)
        {
            if (jToken.Type == JTokenType.Object ||
                jToken.Type == JTokenType.Array ||
                jToken.Type == JTokenType.Property)
                return (string)(new JRaw(jToken));
            return (string)jToken;
        }

        private bool IsTokenNullOrEmpty(JToken token)
        {
            return (token == null) ||
                   (token.Type == JTokenType.Array && !token.HasValues) ||
                   (token.Type == JTokenType.Object && !token.HasValues) ||
                   (token.Type == JTokenType.String && token.ToString() == string.Empty) ||
                   (token.Type == JTokenType.Null);
        }
    }
}
