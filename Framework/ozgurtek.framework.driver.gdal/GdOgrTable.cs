using System;
using System.Collections.Generic;
using System.Data;
using OSGeo.OGR;
using ozgurtek.framework.core.Data;
using Envelope = NetTopologySuite.Geometries.Envelope;

namespace ozgurtek.framework.driver.gdal
{
    public class GdOgrTable : IGdTable
    {
        private readonly Layer _layer;

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

        public IEnumerable<IGdRow> Rows { get; }
        public Envelope Envelope { get; }
        public GdGeometryType? GeometryType { get; set; }
        public int Srid { get; set; }
        public string GeometryField { get; set; }
        
        public string Description { get; set; }
        public string Address { get; set; }
        public string KeyField { get; set; }
        public IGdSchema Schema { get; }
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
