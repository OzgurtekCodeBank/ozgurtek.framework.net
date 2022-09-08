using System;
using System.Collections.Generic;
using OSGeo.OGR;
using OSGeo.OSR;
using ozgurtek.framework.common.Data;
using ozgurtek.framework.common.Data.Format;
using ozgurtek.framework.core.Data;
using Envelope = NetTopologySuite.Geometries.Envelope;
using Layer = OSGeo.OGR.Layer;

namespace ozgurtek.framework.driver.gdal
{
    public class GdOgrTable : IGdTable
    {
        private readonly Layer _layer;
        private readonly string _address;
        private IGdSchema _schema;
        private string _attributeFilter;
        private IGdGeometryFilter _geometryFilter;
        private string _geometryField;

        public GdOgrTable(Layer layer, string address)
        {
            _layer = layer;
            _address = address;
        }

        public long RowCount
        {
            get { return _layer.GetFeatureCount(1); }
        }

        public string Name
        {
            get { return _layer.GetName(); }
        }

        public IEnumerable<IGdRow> Rows
        {
            get
            {
                _layer.ResetReading();

                Feature feature;
                while ((feature = _layer.GetNextFeature()) != null)
                {
                    GdRowBuffer buffer = new GdRowBuffer();
                    buffer.Table = this;

                    //create fields of buffer
                    FeatureDefn featureDefn = feature.GetDefnRef();
                    for (int i = 0; i < featureDefn.GetFieldCount(); i++)
                    {
                        FieldDefn fieldDefn = featureDefn.GetFieldDefn(i);
                        string fieldName = fieldDefn.GetName();
                        FieldType fieldType = fieldDefn.GetFieldType();

                        //is null
                        if (feature.IsFieldNull(fieldName))
                        {
                            buffer.PutNull(fieldName);
                            continue;
                        }

                        //attribute type
                        switch (fieldType)
                        {
                            case FieldType.OFTDate:
                            case FieldType.OFTDateTime:
                            case FieldType.OFTTime:
                                try
                                {
                                    feature.GetFieldAsDateTime(fieldName, out var year, out var month, out var day,
                                        out var hour, out var minute, out var second, out _);
                                    //DateTime dateTime = new DateTime(year, month, day, hour, minute, (int) second);
                                    buffer.Put(fieldName, DateTime.Now, GdDataType.Date);
                                }
                                catch
                                {
                                    // ignored
                                }

                                break;

                            case FieldType.OFTInteger:
                                buffer.Put(fieldName, feature.GetFieldAsInteger(fieldName), GdDataType.Integer);
                                break;

                            case FieldType.OFTInteger64:
                                buffer.Put(fieldName, feature.GetFieldAsInteger64(fieldName), GdDataType.Integer);
                                break;

                            case FieldType.OFTReal:
                                buffer.Put(fieldName, feature.GetFieldAsDouble(fieldName), GdDataType.Real);
                                break;

                            default:
                                buffer.Put(fieldName, feature.GetFieldAsString(fieldName), GdDataType.String);
                                break;
                        }
                    }

                    //additional geometry
                    int geomFieldCount = feature.GetGeomFieldCount();
                    for (int i = 0; i < geomFieldCount; i++)
                    {
                        GeomFieldDefn geomFieldDefn = feature.GetGeomFieldDefnRef(i);
                        string fieldName = geomFieldDefn.GetName();
                        if (string.IsNullOrWhiteSpace(fieldName))
                            continue;

                        Geometry ogrGeom = feature.GetGeomFieldRef(i);
                        if (ogrGeom == null)
                            continue;

                        byte[] wkbBuffer = new byte[ogrGeom.WkbSize()];
                        ogrGeom.ExportToWkb(wkbBuffer);
                        NetTopologySuite.Geometries.Geometry ntsgeom = DbConvert.ToGeometry(wkbBuffer);
                        buffer.Put(fieldName, ntsgeom, GdDataType.Geometry);
                    }

                    //default fid
                    buffer.Put("gd_fid", feature.GetFID(), GdDataType.Integer);

                    //default geometry
                    Geometry geometry = feature.GetGeometryRef();
                    if (geometry != null)
                    {
                        byte[] wkbBuffer = new byte[geometry.WkbSize()];
                        geometry.ExportToWkb(wkbBuffer);
                        NetTopologySuite.Geometries.Geometry geom = DbConvert.ToGeometry(wkbBuffer);
                        buffer.Put("gd_geom", geom, GdDataType.Geometry);
                    }

                    //default style string
                    string styleString = feature.GetStyleString();
                    if (!string.IsNullOrWhiteSpace(styleString))
                    {
                        buffer.Put("gd_style", styleString, GdDataType.String);
                    }

                    yield return buffer;
                }
            }
        }

        public IGdSchema Schema
        {
            get
            {
                if (_schema != null)
                    return _schema;

                GdSchema schema = new GdSchema();

                FeatureDefn layerDefn = _layer.GetLayerDefn();
                int fieldCount = layerDefn.GetFieldCount();
                for (int i = 0; i < fieldCount; i++)
                {
                    FieldDefn ogrFieldDef = layerDefn.GetFieldDefn(i);

                    GdField field = new GdField();
                    field.FieldName = ogrFieldDef.GetName();
                    field.FieldType = GdOgrUtil.GetDataType(ogrFieldDef.GetFieldType());
                    field.DefaultVal = ogrFieldDef.GetDefault();
                    field.NotNull = !DbConvert.ToBoolean(ogrFieldDef.IsNullable());

                    //find geometry field
                    int geomFieldCount = layerDefn.GetGeomFieldCount();
                    for (int j = 0; j < geomFieldCount; j++)
                    {
                        GeomFieldDefn geomFieldDefn = layerDefn.GetGeomFieldDefn(j);
                        if (geomFieldDefn.GetName().Equals(field.FieldName))
                        {
                            wkbGeometryType wkbGeometryType = geomFieldDefn.GetFieldType();
                            field.GeometryType = GdOgrUtil.GetGeometryType(wkbGeometryType);
                            break;
                        }
                    }

                    schema.Add(field);
                }

                return _schema = schema;
            }
        }

        public Envelope Envelope
        {
            get
            {
                OSGeo.OGR.Envelope envelope = new OSGeo.OGR.Envelope();
                _layer.GetExtent(envelope, 1);
                return new Envelope(envelope.MinX, envelope.MaxX, envelope.MinY, envelope.MaxY);
            }
        }

        public GdGeometryType? GeometryType
        {
            get { return GdOgrUtil.GetGeometryType(_layer.GetGeomType()); }
            set { throw new NotSupportedException("Not Supported"); }
        }

        public int Srid
        {
            get { throw new NotSupportedException("Use ProjectionString method"); }
            set { throw new NotSupportedException("Not Supported"); }
        }

        public string GeometryField
        {
            get { return _geometryField; }
            set { _geometryField = value; }
        }

        public string Description
        {
            get { return Name; }
            set { throw new NotSupportedException("Not Supported"); }
        }

        public string Address
        {
            get { return _address + ":" + Name; }
            set { throw new NotSupportedException("Not Supported"); }
        }

        public string KeyField
        {
            get { return _layer.GetFIDColumn(); }
            set { throw new NotSupportedException("Not Supported"); }
        }

        public IGdRow FindRow(long rowId)
        {
            foreach (IGdRow row in Rows)
            {
                if (row.IsNull("gd_fid"))
                    continue;

                if (row.GetAsInteger("gd_fid") == rowId)
                    return row;
            }

            return null;
        }

        public IEnumerable<object> GetDistinctValues(string fieldName)
        {
            GdDataType dataType = Schema.GetFieldByName(fieldName).FieldType;
            HashSet<object> objects = new HashSet<object>();
            foreach (IGdRow row in Rows)
            {
                if (!row.IsNull(fieldName))
                {
                    if (dataType == GdDataType.String)
                        objects.Add(row.GetAsString(fieldName));
                    else if (dataType == GdDataType.Boolean)
                        objects.Add(row.GetAsBoolean(fieldName));
                    else if (dataType == GdDataType.Date)
                        objects.Add(row.GetAsDate(fieldName));
                    else if (dataType == GdDataType.Integer)
                        objects.Add(row.GetAsInteger(fieldName));
                    else if (dataType == GdDataType.Real)
                        objects.Add(row.GetAsReal(fieldName));
                }
                else
                    objects.Add(null);
            }
            return objects;
        }

        public IGdGeometryFilter GeometryFilter
        {
            get
            {
                return _geometryFilter;
            }
            set
            {
                _geometryFilter = value;

                if (value == null)
                {
                    _layer.SetSpatialFilter(null);
                    return;
                }

                if (value.Envelope != null)
                    _layer.SetSpatialFilterRect(value.Envelope.MinX, value.Envelope.MinY, value.Envelope.MaxX,
                        value.Envelope.MaxY);

                if (value.Geometry != null)
                {
                    string wkt = value.Geometry.ToText();
                    Geometry geometryFromWkt = Ogr.CreateGeometryFromWkt(ref wkt, _layer.GetSpatialRef());
                    _layer.SetSpatialFilter(geometryFromWkt);
                }
            }
        }

        public long Insert(IGdRowBuffer row)
        {
            throw new NotSupportedException();
        }

        public long Update(IGdRowBuffer row)
        {
            throw new NotSupportedException();
        }

        public long Delete(long id)
        {
            throw new NotSupportedException();
        }

        public void Truncate()
        {
            throw new NotSupportedException();
        }

        public bool CanEditRow
        {
            get { return false; }
        }

        public void CreateField(IGdField field)
        {
            throw new NotSupportedException();
        }

        public void DeleteField(IGdField field)
        {
            throw new NotSupportedException();
        }

        public bool CanEditField
        {
            get { return false; }
        }

        public string ToGeojson(GdGeoJsonSeralizeType type, int dimension = 2)
        {
            GdGeoJsonSerializer serializer = new GdGeoJsonSerializer();
            serializer.SerializeType = GdGeoJsonSeralizeType.All;
            serializer.Dimension = dimension;
            return serializer.Serialize(this);
        }

        public IGdTable Clone()
        {
            GdMemoryTable table = GdMemoryTable.LoadFromTable(this);
            return table;
        }

        public override string ToString()
        {
            return Name;
        }

        public event EventHandler<GdRowChangedEventArgs> RowChanged;

        public string ProjectionString
        {
            get
            {
                SpatialReference spatialReference = _layer.GetSpatialRef();
                if (spatialReference == null)
                    return null;

                spatialReference.ExportToWkt(out var wkt, null);
                return wkt;
            }
        }

        public string AttributeFilter
        {
            get
            {
                return _attributeFilter;
            }
            set
            {
                _attributeFilter = value;
                _layer.SetAttributeFilter(value);
            }
        }
    }
}