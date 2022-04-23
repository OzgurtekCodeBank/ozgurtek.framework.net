using NetTopologySuite.Geometries;
using ozgurtek.framework.common.Data;
using ozgurtek.framework.common.Geodesy;
using ozgurtek.framework.common.Style;
using ozgurtek.framework.common.Util;
using ozgurtek.framework.core.Data;
using ozgurtek.framework.core.Mapping;
using ozgurtek.framework.core.Style;
using System;

namespace ozgurtek.framework.common.Mapping
{
    public class GdSimpleFeatureRenderer : IGdRenderer
    {
        private readonly IGdFeatureLayer _layer;
        private readonly GdRenderMode _mode;
        private IGdStyle _style;

        public GdSimpleFeatureRenderer(IGdFeatureLayer layer, GdRenderMode mode)
        {
            _layer = layer;
            _mode = mode;
            InitRenderer();
        }

        private void InitRenderer()
        {
            if (string.IsNullOrWhiteSpace(_layer.Table.GeometryField))
                return;

            if (_layer.Table.GeometryType == null)
                return;

            if (_mode == GdRenderMode.Geometry)
            {
                if (_layer.Table.GeometryType == GdGeometryType.Polygon)
                    _style = GdPolygonStyle.Default;
                else if (_layer.Table.GeometryType == GdGeometryType.Line)
                    _style = GdLineStyle.Default;
                else if (_layer.Table.GeometryType == GdGeometryType.Point)
                    _style = GdPointStyle.Default;
            }
            else if (_mode == GdRenderMode.Label)
                _style = GdTextStyle.Default;
        }

        public void Render(IGdRenderContext context, IGdTrack track = null)
        {
            if (_style == null)
                return;

            long counter = 0;
            long partialCounter = 1000;
            const long maxCounter = 10000;

            string columnFilter = null;
            IGdGeometryFilter geometryFilter = null;
            IGdTable table = _layer.Table;

            if (context.Viewport.Srid <= 0)
                throw new Exception("map srid wrong");

            if (_layer.Table.Srid <= 0)
                throw new Exception($"table {_layer.Name} srid wrong");

            string geometryField = table.GeometryField;
            IGdDbTable dbTable = table as IGdDbTable;
            if (dbTable != null)
            {
                if (_mode != GdRenderMode.Label)
                {
                    columnFilter = dbTable.ColumnFilter;
                    dbTable.ColumnFilter = geometryField;
                }

                geometryFilter = dbTable.GeometryFilter;
                Envelope envelope = GdProjection.Project(context.Viewport.World, context.Viewport.Srid, table.Srid);
                dbTable.GeometryFilter = new GdGeometryFilter(envelope);
            }

            foreach (IGdRow row in table.Rows)
            {
                if (track != null && track.CancellationPending)
                    break;

                if (row == null)
                    continue;

                Geometry geometry = row.GetAsGeometry(geometryField);
                if (geometry == null || geometry.SRID <= 0)
                    continue;

                GdGeometryType? geometryType = GdGeometryUtil.ConvertGeometryType(geometry.OgcGeometryType);
                if (!geometryType.HasValue || !_layer.Table.GeometryType.HasValue || geometryType != _layer.Table.GeometryType)
                    continue;

                if (_mode == GdRenderMode.Geometry)
                    RenderGeometry(geometry, context);
                else if (_mode == GdRenderMode.Label)
                    RenderLabel(row, geometry, context);

                if (++counter > partialCounter)
                {
                    counter = 0;
                    if (track != null && !track.CancellationPending)
                        context.Flush();

                    if (partialCounter < maxCounter)
                        partialCounter *= 2;
                    else
                        partialCounter = maxCounter;
                }
            }

            if (track != null && !track.CancellationPending && counter > 0)
                context.Flush();

            if (dbTable != null)
            {
                if (_mode != GdRenderMode.Label)
                {
                    dbTable.ColumnFilter = columnFilter;
                }

                dbTable.GeometryFilter = geometryFilter;
            }
        }

        private void RenderGeometry(Geometry geometry, IGdRenderContext context)
        {
            _style.Render(context, geometry);
        }

        private void RenderLabel(IGdRow row, Geometry geometry, IGdRenderContext context)
        {
            IGdLabeledLayer labeledLayer = _layer as IGdLabeledLayer;
            if (labeledLayer == null)
                return;

            string label = labeledLayer.LabelFormat;
            foreach (IGdField field in row.Table.Schema.Fields)
            {
                string find = $"[{field.FieldName}]";

                string replace = "null";
                if (!row.IsNull(field.FieldName))
                    replace = row.GetAsString(field.FieldName);

                label = label.Replace(find, replace);
            }

            Style.Render(context, geometry, label);
        }

        public IGdStyle Style
        {
            get => _style;
            set => _style = value;
        }
    }
}