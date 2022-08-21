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

                    foreach (IGdField field in _schema.Fields)
                    {
                        if (field.FieldType == GdDataType.Integer)
                            buffer.Put(field.FieldName, feature.GetFieldAsInteger64(field.FieldName));
                        if (field.FieldType == GdDataType.Real)
                            buffer.Put(field.FieldName, feature.GetFieldAsDouble(field.FieldName));
                        else
                            buffer.Put(field.FieldName, feature.GetFieldAsString(field.FieldName));
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

                FeatureDefn ogrFeatureDef = _layer.GetLayerDefn();
                int fieldCount = ogrFeatureDef.GetFieldCount();
                for (int i = 0; i < fieldCount; i++)
                {
                    FieldDefn ogrFieldDef = ogrFeatureDef.GetFieldDefn(i);
                    
                    GdField field = new GdField();
                    field.FieldName = ogrFieldDef.GetName();
                    field.FieldType = GetDataType(ogrFieldDef.GetFieldType());
                    field.DefaultVal = ogrFieldDef.GetDefault();
                    field.NotNull = !DbConvert.ToBoolean(ogrFieldDef.IsNullable());

                    //find geometry field
                    int geomFieldCount = ogrFeatureDef.GetGeomFieldCount();
                    for (int j = 0; j < geomFieldCount; j++)
                    {
                        GeomFieldDefn geomFieldDefn = ogrFeatureDef.GetGeomFieldDefn(j);
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

        private GdDataType GetDataType(FieldType fieldType)
        {
            switch (fieldType)
            {
                case FieldType.OFTDate:
                case FieldType.OFTDateTime:
                case FieldType.OFTTime:
                    return GdDataType.Date;

                case FieldType.OFTBinary:
                    return GdDataType.Boolean;

                case FieldType.OFTInteger:
                case FieldType.OFTInteger64:
                    return GdDataType.Integer;

                case FieldType.OFTReal:
                    return GdDataType.Real;

                default:
                    return GdDataType.String;
            }
        }
    }
}
