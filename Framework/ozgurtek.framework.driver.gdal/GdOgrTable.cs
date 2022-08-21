using System;
using System.Collections.Generic;
using System.Data;
using OSGeo.OGR;
using ozgurtek.framework.common.Data;
using ozgurtek.framework.core.Data;
using Envelope = NetTopologySuite.Geometries.Envelope;

namespace ozgurtek.framework.driver.gdal
{
    public class GdOgrTable : IGdTable
    {
        private readonly Layer _layer;
        private IGdSchema _schema;

        public GdOgrTable(Layer layer)
        {
            _layer = layer;
        }

        public int RowCount
        {
            get
            {
                return (int)_layer.GetFeatureCount(1);
            }
        }

        public string Name
        {
            get
            {
                return _layer.GetName();
            }
        }

        public IEnumerable<IGdRow> Rows
        {
            get
            {
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

                        if (feature.IsFieldNull(fieldName))
                        {
                            buffer.PutNull(fieldName);
                            continue;
                        }

                        switch (fieldType)
                        {
                            case FieldType.OFTDate:
                            case FieldType.OFTDateTime:
                            case FieldType.OFTTime:
                                try
                                {
                                    feature.GetFieldAsDateTime(fieldName, out var year, out var month, out var day, out var hour, out var minute, out var second, out _);
                                    DateTime dateTime = new DateTime(year, month, day, hour, minute, (int)second);
                                    buffer.Put(fieldName, dateTime, GdDataType.Date);
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

                    //fid
                    buffer.Put("gd_fid", feature.GetFID(), GdDataType.Integer);
                    
                    //geometry
                    Geometry geometry = feature.GetGeometryRef();
                    if (geometry != null)
                    {
                        byte[] wkbBuffer = new byte[geometry.WkbSize()];
                        geometry.ExportToWkb(wkbBuffer);
                        NetTopologySuite.Geometries.Geometry geom = DbConvert.ToGeometry(wkbBuffer);
                        buffer.Put("gd_geom", geom, GdDataType.Geometry);
                    }

                    //style string
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

        public Envelope Envelope { get; }
        public GdGeometryType? GeometryType { get; set; }
        public int Srid { get; set; }
        public string GeometryField { get; set; }
        
        public string Description { get; set; }
        public string Address { get; set; }
        public string KeyField { get; set; }
        
        public IGdRow FindRow(long rowId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<object> GetDistinctValues(string fieldName)
        {
            throw new NotImplementedException();
        }

        public IGdGeometryFilter GeometryFilter { get; set; }
        public long Insert(IGdRowBuffer row)
        {
            throw new NotImplementedException();
        }

        public long Update(IGdRowBuffer row)
        {
            throw new NotImplementedException();
        }

        public long Delete(long id)
        {
            throw new NotImplementedException();
        }

        public void Truncate()
        {
            throw new NotImplementedException();
        }

        public bool CanEditRow { get; }
        public void CreateField(IGdField field)
        {
            throw new NotImplementedException();
        }

        public void DeleteField(IGdField field)
        {
            throw new NotImplementedException();
        }

        public bool CanEditField { get; }
        public string ToGeojson(GdGeoJsonSeralizeType type)
        {
            throw new NotImplementedException();
        }

        public DataTable ToDataTable()
        {
            throw new NotImplementedException();
        }

        public IGdTable Clone()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return Name;
        }

        public event EventHandler<GdRowChangedEventArgs> RowChanged;
    }
}
