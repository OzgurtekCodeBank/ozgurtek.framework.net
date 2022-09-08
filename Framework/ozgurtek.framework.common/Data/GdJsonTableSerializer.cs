using NetTopologySuite.Geometries;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ozgurtek.framework.core.Data;

namespace ozgurtek.framework.common.Data
{
    public class GdGeoJsonSerializer
    {
        private GdGeoJsonSeralizeType _serializeType = GdGeoJsonSeralizeType.All;
        private int _dimension = 2;
        private GeometryFactory _geometryFactory = GeometryFactory.Default;

        public GdGeoJsonSeralizeType SerializeType
        {
            get { return _serializeType; }
            set { _serializeType = value; }
        }

        public int Dimension
        {
            get => _dimension;
            set => _dimension = value;
        }

        public GeometryFactory GeometryFactory
        {
            get => _geometryFactory;
            set => _geometryFactory = value;
        }

        public string Serialize(IGdTable table)
        {
            JObject tableJson = new JObject();
            tableJson.Add(new JProperty("type", "FeatureCollection"));
            tableJson.Add(new JProperty("name", table.Name));
            tableJson.Add(new JProperty("description", table.Description));
            tableJson.Add(new JProperty("keyfield", table.KeyField));
            tableJson.Add(new JProperty("geometryfield", table.GeometryField));
            
            SerializeMetaData(table, tableJson);
            SerializeData(table, tableJson);

            return tableJson.ToString(Formatting.None);
        }

        private void SerializeMetaData(IGdTable table, JObject tableJson)
        {
            if (_serializeType == GdGeoJsonSeralizeType.OnlyData)
                return;

            JArray fieldsJson = new JArray();
            IGdSchema schema = table.Schema;
            foreach (IGdField field in schema.Fields)
            {
                if (field.FieldName == table.GeometryField)
                    continue;

                JObject fieldPropropertiesJson = new JObject();
                fieldPropropertiesJson.Add("name", field.FieldName);
                fieldPropropertiesJson.Add("type", field.FieldType.ToString());
                fieldsJson.Add(fieldPropropertiesJson);
            }
            tableJson.Add(new JProperty("fields", fieldsJson));
        }

        private void SerializeData(IGdTable table, JObject tableJson)
        {
            if (_serializeType == GdGeoJsonSeralizeType.OnlyMetaData)
                return;

            //features
            JArray featuresJson = new JArray();

            IGdSchema schema = table.Schema;
            string geometryField = table.GeometryField;
            foreach (IGdRow row in table.Rows)
            {
                JObject feature = new JObject();
                feature.Add("type", "Feature");

                //geometry                
                if (!string.IsNullOrEmpty(geometryField) && !row.IsNull(geometryField))
                {
                    Geometry geometry = row.GetAsGeometry(geometryField);
                    string json = DbConvert.ToJson(geometry, GeometryFactory, Dimension);
                    JObject jObject = JObject.Parse(json);
                    JProperty property = new JProperty("geometry", jObject);
                    feature.Add(property);
                }

                //properties
                JObject propertiesJson = new JObject();
                foreach (IGdField field in schema.Fields)
                {
                    if (field.FieldName == geometryField)
                        continue;

                    object value = null;
                    if (!row.IsNull(field.FieldName))
                    {
                        GdDataType dataType = field.FieldType;
                        switch (dataType)
                        {
                            case GdDataType.Boolean:
                                value = row.GetAsBoolean(field.FieldName);
                                break;
                            case GdDataType.Date:
                                value = row.GetAsDate(field.FieldName);
                                break;
                            case GdDataType.Integer:
                                value = row.GetAsInteger(field.FieldName);
                                break;
                            case GdDataType.Real:
                                value = row.GetAsReal(field.FieldName);
                                break;
                            case GdDataType.String:
                            case GdDataType.Blob:
                            case GdDataType.Geometry:
                                value = row.GetAsString(field.FieldName);
                                break;
                            default:
                                continue;
                        }
                    }
                    JProperty property = new JProperty(field.FieldName, value);
                    propertiesJson.Add(property);
                }
                feature.Add("properties", propertiesJson);

                featuresJson.Add(feature);
            }
            tableJson.Add(new JProperty("features", featuresJson));
        }
    }
}