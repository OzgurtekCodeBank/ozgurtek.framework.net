using NetTopologySuite.Geometries;
using ozgurtek.framework.common.Geodesy;
using System;
using System.Globalization;
using ozgurtek.framework.common.Data;
using ozgurtek.framework.core.Data;

namespace ozgurtek.framework.driver.sqlite
{
    internal class GdSqliteQueryBuilder
    {
        private readonly GdSqlLiteTable _table;

        public GdSqliteQueryBuilder(GdSqlLiteTable table)
        {
            _table = table;
        }

        public virtual string CreateQuery()
        {
            string filter = string.Empty;
            AppendBaseQuery(ref filter);
            AppendSqlFilter(ref filter);
            AppendGeometryFilter(ref filter);
            AppendLast(ref filter);
            return filter;
        }

        protected virtual void AppendBaseQuery(ref string filter)
        {
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));

            filter = $"SELECT * FROM ({_table.BaseFilter.Text}) A";

            if (_table.BaseFilter == null) 
                return;
            
            foreach (IGdParamater filterParams in _table.BaseFilter.Parameters)
            {
                string replace = DbConvert.IsDbNull(filterParams.Value) ? "null" : filterParams.Value.ToString();
                filter = filter.Replace("@"+filterParams.Name, replace);
            }
        }

        protected virtual void AppendSqlFilter(ref string filter)
        {
            if (_table.SqlFilter == null)
                return;

            filter = $"SELECT * FROM ({filter}) A WHERE {_table.SqlFilter.Text}";

            foreach (IGdParamater filterParams in _table.SqlFilter.Parameters)
            {
                string replace = DbConvert.IsDbNull(filterParams.Value) ? "null" : filterParams.Value.ToString();
                filter = filter.Replace("@" + filterParams.Name, replace);
            }
        }

        private void AppendGeometryFilter(ref string filter)
        {
            if (_table.GeometryFilter == null)
                return;

            Envelope envelope;
            if (_table.GeometryFilter.Geometry != null)
            {
                Geometry geometry = GdProjection.Project(_table.GeometryFilter.Geometry, _table.Srid);
                envelope = geometry.EnvelopeInternal;
            }
            else
            {
                envelope = _table.GeometryFilter.Envelope;
            }

            filter = "select ain.* from " +
                     "geometry_index idxt, " +
                     $"({filter}) ain " +
                     "where ain.ogc_fid = idxt.pkid and " +
                     "table_column =" +
                     $"'{_table.Name + "_" + _table.GeometryField}' and " +
                     $"{envelope.MinX.ToString(CultureInfo.InvariantCulture)} <= idxt.xmax and " +
                     $"{envelope.MaxX.ToString(CultureInfo.InvariantCulture)} >= idxt.xmin and " +
                     $"{envelope.MinY.ToString(CultureInfo.InvariantCulture)} <= idxt.ymax and " +
                     $"{envelope.MaxY.ToString(CultureInfo.InvariantCulture)} >= idxt.ymin";
        }

        protected virtual void AppendLast(ref string filter)
        {
            string columnFilter = "*";
            if (!string.IsNullOrEmpty(_table.ColumnFilter))
                columnFilter = _table.ColumnFilter;

            string orderBy = "";
            if (!string.IsNullOrEmpty(_table.OrderBy))
                orderBy = " ORDER BY " + _table.OrderBy;

            string limit = "";
            if (_table.Limit >= 0)
                limit = " LIMIT " + _table.Limit;

            string offset = "";
            if (_table.Offset >= 0)
                offset = " OFFSET " + _table.Offset;

            filter = $"SELECT {columnFilter} FROM ({filter}) A {orderBy} {limit} {offset}";
        }
    }
}
