using System;
using System.Collections.Generic;
using System.Linq;
using GeoAPI.CoordinateSystems;
using OSGeo.OGR;
using OSGeo.OSR;
using ozgurtek.framework.common.Data;
using ozgurtek.framework.common.Data.Format.Wmts;
using ozgurtek.framework.core.Data;
using Envelope = NetTopologySuite.Geometries.Envelope;
using Layer = OSGeo.OGR.Layer;

namespace ozgurtek.framework.driver.gdal
{
    public class GdOgrTable : GdAbstractTable
    {
        private readonly Layer _layer;
        private IGdSchema _schema;
        private string _attributeFilter;
        private IGdGeometryFilter _geometryFilter;

        public GdOgrTable(Layer layer, string address)
        {
            _layer = layer;
            Name = GdOgrUtil.ToOgrString(layer.GetName());
            Address = address + ":" + Name;
            KeyField = GdOgrUtil.ToOgrString(_layer.GetFIDColumn());
            GeometryField = GdOgrUtil.ToOgrString(_layer.GetGeometryColumn());
            GeometryType = GdOgrUtil.GetGeometryType(_layer.GetGeomType());
        }

        public override long RowCount
        {
            get
            {
                try
                {
                    return _layer.GetFeatureCount(1);
                }
                catch
                {
                    return base.RowCount;
                }
            }
        }

        public override IEnumerable<IGdRow> Rows
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
                                buffer.Put(fieldName, GdOgrUtil.ToOgrString(feature.GetFieldAsString(fieldName)), GdDataType.String);
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
                            fieldName = "gd_geom_ext" + i;

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

        public override IGdSchema Schema
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
                    field.FieldName = GdOgrUtil.ToOgrString(ogrFieldDef.GetName());
                    field.FieldType = GdOgrUtil.GetDataType(ogrFieldDef.GetFieldType());
                    field.DefaultVal = ogrFieldDef.GetDefault();
                    field.NotNull = !DbConvert.ToBoolean(ogrFieldDef.IsNullable());

                    schema.Add(field);
                }

                return _schema = schema;
            }
        }

        public override IGdTable Clone()
        {
            return null;
        }

        public override Envelope Envelope
        {
            get
            {
                try
                {
                    OSGeo.OGR.Envelope envelope = new OSGeo.OGR.Envelope();
                    _layer.GetExtent(envelope, 1);
                    return new Envelope(envelope.MinX, envelope.MaxX, envelope.MinY, envelope.MaxY);
                }
                catch
                {
                    return base.Envelope;
                }
            }
        }

        public override IGdGeometryFilter GeometryFilter
        {
            get { return _geometryFilter; }
            set
            {
                _geometryFilter = value;

                if (value == null)
                {
                    _layer.SetSpatialFilter(null);
                    return;
                }

                if (value.Envelope != null)
                    _layer.SetSpatialFilterRect(value.Envelope.MinX, 
                        value.Envelope.MinY, 
                        value.Envelope.MaxX,
                        value.Envelope.MaxY);

                if (value.Geometry != null)
                {
                    string wkt = value.Geometry.ToText();
                    Geometry geometryFromWkt = Ogr.CreateGeometryFromWkt(ref wkt, _layer.GetSpatialRef());
                    _layer.SetSpatialFilter(geometryFromWkt);
                }
            }
        }

        public override bool CanEditField
        {
            get { return true; }
        }

        public override void CreateField(IGdField field)
        {
            FieldType fieldType = GdOgrUtil.GetDataType(field.FieldType);
            FieldDefn defn = new FieldDefn(field.FieldName, fieldType);
            _layer.CreateField(defn, 0);
        }

        public override void DeleteField(IGdField field)
        {
            throw new NotImplementedException();
        }

        public override long Insert(IGdRowBuffer row)
        {
            FeatureDefn featureDefn = new FeatureDefn("");
            foreach (IGdParamater paramater in row.Paramaters)
            {
                if (paramater.Name.Equals(GeometryField, StringComparison.OrdinalIgnoreCase))
                    continue;

                GdDataType? type = paramater.DataType ?? GdDataType.String;
                FieldType fieldType = GdOgrUtil.GetDataType(type.Value);
                FieldDefn defn = new FieldDefn(paramater.Name, fieldType);
                featureDefn.AddFieldDefn(defn);
            }

            Feature feature = new Feature(featureDefn);
            foreach (IGdParamater paramater in row.Paramaters)
            {
                if (paramater.Name.Equals(GeometryField, StringComparison.OrdinalIgnoreCase))
                {
                    NetTopologySuite.Geometries.Geometry geom = row.GetAsGeometry(paramater.Name);
                    Geometry wkb = Geometry.CreateFromWkb(geom.ToBinary());
                    feature.SetGeometryDirectly(wkb);
                    continue;
                }

                GdDataType? type = paramater.DataType ?? GdDataType.String;
                switch (type)
                {
                    case GdDataType.Boolean:
                        feature.SetField(paramater.Name, row.GetAsInteger(paramater.Name));
                        break;
                    case GdDataType.Real:
                        feature.SetField(paramater.Name, row.GetAsReal(paramater.Name));
                        break;
                    case GdDataType.String:
                        feature.SetField(paramater.Name, row.GetAsString(paramater.Name));
                        break;
                    case GdDataType.Integer:
                        feature.SetField(paramater.Name, row.GetAsInteger(paramater.Name));
                        break;
                    case GdDataType.Blob:
                        feature.SetFieldBinaryFromHexString(paramater.Name, row.GetAsString(paramater.Name));
                        break;
                    case GdDataType.Geometry:
                        byte[] bytes = DbConvert.ToWkb(row.GetAsGeometry(paramater.Name));
                        feature.SetFieldBinaryFromHexString(paramater.Name, DbConvert.ToString(bytes));
                        break;
                    case GdDataType.Date:
                        DateTime dt = DbConvert.ToDateTime(paramater.Value);
                        feature.SetField(paramater.Name, dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, 0);
                        break;
                }
            }
            return _layer.CreateFeature(feature);
        }

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
            get { return _attributeFilter; }
            set
            {
                _attributeFilter = value;
                _layer.SetAttributeFilter(value);
            }
        }

        public void Save()
        {
            _layer.SyncToDisk();
        }
    }
}