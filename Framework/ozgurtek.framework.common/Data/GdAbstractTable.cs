using NetTopologySuite.Geometries;
using ozgurtek.framework.common.Util;
using ozgurtek.framework.core.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ozgurtek.framework.common.Data
{
    public abstract class GdAbstractTable : IGdTable
    {
        private int? _srid;
        private string _geomeryField;
        private GdGeometryType? _geometryType;
        private IGdGeometryFilter _geometryFilter;

        public virtual string Name { get; set; }

        public virtual string Address { get; set; }

        public virtual string Description { get; set; }

        public virtual string KeyField { get; set; }

        public virtual string GeometryField
        {
            get
            {
                if (_geomeryField != null)
                    return _geomeryField;

                foreach (IGdField field in Schema.Fields)
                {
                    if (field.FieldType == GdDataType.Geometry)
                        return _geomeryField = field.FieldName;
                }
                return null;
            }
            set { _geomeryField = value; }
        }

        public virtual Envelope Envelope
        {
            get
            {
                if (string.IsNullOrWhiteSpace(GeometryField))
                    throw new Exception("GeometryField Not Set");

                Envelope envelope = new Envelope();
                foreach (IGdRow row in Rows)
                {
                    if (row.IsNull(GeometryField))
                        continue;

                    Geometry geometry = row.GetAsGeometry(GeometryField);
                    if (geometry == null)
                        continue;

                    envelope.ExpandToInclude(geometry.EnvelopeInternal);
                }

                return envelope;
            }
        }

        public GdGeometryType? GeometryType
        {
            get
            {
                if (string.IsNullOrWhiteSpace(GeometryField))
                    return null;

                if (_geometryType.HasValue)
                    return _geometryType.Value;

                HashSet<GdGeometryType> geometryTypes = new HashSet<GdGeometryType>();
                foreach (IGdRow row in Rows)
                {
                    if (row.IsNull(GeometryField))
                        continue;

                    Geometry geometry = row.GetAsGeometry(GeometryField);
                    if (geometry == null)
                        continue;

                    GdGeometryType? geometryType = GdGeometryUtil.ConvertGeometryType(geometry.OgcGeometryType);
                    if (geometryType.HasValue)
                        geometryTypes.Add(geometryType.Value);

                    if (geometryTypes.Count > 1)
                        break;
                }

                if (geometryTypes.Count == 1)
                {
                    _geometryType = geometryTypes.FirstOrDefault();
                    return _geometryType.Value;
                }

                _geometryType = null;
                return _geometryType;
            }
            set { _geometryType = value; }
        }

        public int Srid
        {
            get
            {
                if (_srid.HasValue)
                    return _srid.Value;

                if (string.IsNullOrWhiteSpace(GeometryField))
                    throw new Exception("GeometryField Not Set");

                HashSet<int> srids = new HashSet<int>();//todo: gerçekten bitane mi alıyor...
                foreach (IGdRow row in Rows)
                {
                    if (row.IsNull(GeometryField))
                        continue;

                    Geometry geometry = row.GetAsGeometry(GeometryField);
                    if (geometry == null)
                        continue;

                    srids.Add(geometry.SRID);
                    if (srids.Count > 1)
                        break;
                }

                if (srids.Count == 1)
                {
                    _srid = srids.FirstOrDefault();
                    return _srid.Value;
                }

                _srid = 0;
                return _srid.Value;
            }
            set { _srid = value; }
        }

        public virtual int RowCount
        {
            get { return Rows.Count(); }
        }

        public virtual IGdRow FindRow(long rowId)
        {
            if (string.IsNullOrWhiteSpace(KeyField))
                throw new Exception("KeyFieldMissing....");

            foreach (IGdRow row in Rows)
            {
                int fid = row.GetAsInteger(KeyField);
                if (fid == rowId)
                    return row;
            }

            return null;
        }

        public virtual IEnumerable<object> GetDistinctValues(string fieldName)
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
            get => _geometryFilter;
            set => _geometryFilter = value;
        }

        public virtual long Insert(IGdRowBuffer row)
        {
            throw new NotSupportedException();
        }

        public virtual long Update(IGdRowBuffer row)
        {
            throw new NotSupportedException();
        }

        public virtual long Delete(long id)
        {
            throw new NotSupportedException();
        }

        public virtual void Truncate()
        {
            throw new NotSupportedException();
        }

        public virtual void CreateField(IGdField field)
        {
            throw new NotSupportedException();
        }

        public virtual void DeleteField(IGdField field)
        {
            throw new NotSupportedException();
        }

        public virtual string ToGeojson(GdGeoJsonSeralizeType type)
        {
            GdGeoJsonSerializer serializer = new GdGeoJsonSerializer();
            serializer.SerializeType = type;
            return serializer.Serialize(this);
        }

        public virtual bool CanEditRow
        {
            get { return false; }
        }

        public virtual bool CanEditField
        {
            get { return false; }
        }

        public override string ToString()
        {
            return Name;
        }

        protected virtual void OnRowChanged(string type, IGdRowBuffer buffer)
        {
            RowChanged?.Invoke(this, new GdRowChangedEventArgs(this, type, buffer));
        }

        protected virtual bool HasRowChangeSubscriptions
        {
            get { return RowChanged != null; }
        }

        public event EventHandler<GdRowChangedEventArgs> RowChanged;
        public abstract IEnumerable<IGdRow> Rows { get; }
        public abstract IGdSchema Schema { get; }
        public abstract IGdTable Clone();
    }
}