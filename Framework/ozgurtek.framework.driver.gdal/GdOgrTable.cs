using System;
using System.Collections.Generic;
using GeoAPI.CoordinateSystems;
using OSGeo.OGR;
using OSGeo.OSR;
using ozgurtek.framework.common.Data;
using ozgurtek.framework.common.Util;
using ozgurtek.framework.core.Data;
using Envelope = NetTopologySuite.Geometries.Envelope;
using Layer = OSGeo.OGR.Layer;

namespace ozgurtek.framework.driver.gdal
{
    public class GdOgrTable : GdAbstractTable
    {
        private readonly Layer _layer;
        private readonly bool _allowMultigeom;
        private IGdSchema _schema;
        private string _attributeFilter;
        private IGdGeometryFilter _geometryFilter;

        public GdOgrTable(Layer layer, string address, bool allowMultigeom = false)
        {
            _layer = layer;
            _allowMultigeom = allowMultigeom;
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
                                buffer.Put(fieldName, GdOgrUtil.ToOgrString(feature.GetFieldAsString(fieldName)),
                                    GdDataType.String);
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

        public override bool CanEditRow
        {
            get { return true; }
        }

        public override void CreateField(IGdField field)
        {
            if (field.FieldType == GdDataType.Geometry)
                throw new NotSupportedException("Geometry type not supported use GdOgrRowBuffer.SetGeometryDirectly()");

            if (field.FieldType != GdDataType.String)
                throw new Exception("Only string field supported...");

            FieldDefn defn = new FieldDefn(field.FieldName, FieldType.OFTString);
            _layer.CreateField(defn, 0);
        }

        public override void DeleteField(IGdField field)
        {
            List<IGdField> fields = new List<IGdField>(Schema.Fields);
            for (int i = 0; i < fields.Count; i++)
            {
                if (field.FieldName.Equals(fields[i].FieldName))
                {
                    _layer.DeleteField(i);
                    return;
                }
            }

            throw new Exception("Field not found");
        }

        public override long Insert(IGdRowBuffer row)
        {
            Feature ogrFeature = GetOgrFeature(row);
            return _layer.CreateFeature(ogrFeature);
        }

        public override long Update(IGdRowBuffer row)
        {
            Feature ogrFeature = GetOgrFeature(row);
            return _layer.UpsertFeature(ogrFeature);
        }

        public override long Delete(long id)
        {
            return _layer.DeleteFeature(id);
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

        public void SyncToDisk()
        {
            _layer.SyncToDisk();
        }

        public void Dispose()
        {
            _layer.Dispose();
        }

        public void StartTransaction()
        {
            _layer.StartTransaction();
        }

        public void CommitTransaction()
        {
            _layer.CommitTransaction();
        }

        public void RollbackTransaction()
        {
            _layer.RollbackTransaction();
        }

        public Layer OgrLayer
        {
            get { return _layer; }
        }

        private Feature GetOgrFeature(IGdRowBuffer row)
        {
            if (!(row is GdOgrRowBuffer ogrRowBuffer))
                throw new NotSupportedException("Use GdOgrRowBuffer class...");

            List<Tuple<string, string>> values = new List<Tuple<string, string>>();
            
            //create feature defination
            FeatureDefn featureDefn = new FeatureDefn("");
            foreach (IGdParamater paramater in ogrRowBuffer.Paramaters)
            {
                if (paramater.Value is NetTopologySuite.Geometries.Geometry)
                    throw new NotSupportedException("Geometry parameter not supported use GdOgrRowBuffer.SetGeometryDirectly()");

                if (!DbConvert.IsDbNull(paramater.Value) && !(paramater.Value is string))
                    throw new NotSupportedException("Only string values supported");

                //accept only string  type
                FieldDefn defn = new FieldDefn(paramater.Name, FieldType.OFTString);
                featureDefn.AddFieldDefn(defn);

                values.Add(DbConvert.IsDbNull(paramater.Value)
                    ? new Tuple<string, string>(paramater.Name, null)
                    : new Tuple<string, string>(paramater.Name, paramater.Value.ToString()));
            }
            
            Feature feature = new Feature(featureDefn);

            //geometry directly
            NetTopologySuite.Geometries.Geometry geometryDirectly = ogrRowBuffer.GetGeometryDirectly();
            if (geometryDirectly != null)
            {
                Geometry wkb = Geometry.CreateFromWkb(geometryDirectly.ToBinary());
                feature.SetGeometryDirectly(wkb);
            }

            //featureid directly
            long? featureIdDirectly = ogrRowBuffer.GetFeatureIdDirectly();
            if (featureIdDirectly.HasValue)
                feature.SetFID(featureIdDirectly.Value);

            //style string directly
            string styleStringDirectly = ogrRowBuffer.GetStyleStringDirectly();
            if (string.IsNullOrWhiteSpace(styleStringDirectly))
                feature.SetStyleString(styleStringDirectly);

            //set fature's field
            foreach (Tuple<string, string> tuple in values)
            {
                string fieldName = tuple.Item1;
                string value = tuple.Item2;

                if (value == null)
                {
                    feature.SetFieldNull(fieldName);
                    continue;
                }

                feature.SetField(fieldName, value);
            }
            
            return feature;
        }
    }
}