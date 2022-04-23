using NetTopologySuite.Geometries;
using ozgurtek.framework.core.Mapping;
using ozgurtek.framework.core.Style;

namespace ozgurtek.framework.common.Style
{
    public class GdPolygonStyle : GdAbstractStyle, IGdPolygonStyle
    {
        private IGdStroke _stroke = new GdStroke();
        private IGdFill _fill = new GdFill();

        public GdPolygonStyle()
        {
        }

        public GdPolygonStyle(IGdStroke stroke, IGdFill fill)
        {
            _stroke = stroke;
            _fill = fill;
        }

        public IGdStroke Stroke
        {
            get { return _stroke; }
            set { _stroke = value; }
        }

        public IGdFill Fill
        {
            get { return _fill; }
            set { _fill = value; }
        }

        public override void Render(IGdRenderContext context, Geometry geometry, object options = null)
        {
            if (geometry == null)
                return;

            int numGeometries = geometry.NumGeometries;
            for (int i = 0; i < numGeometries; i++)
            {
                Geometry thisGeom = geometry.GetGeometryN(i);
                if (thisGeom.NumGeometries > 1)
                    Render(context, thisGeom, options);

                if (thisGeom is Polygon polygon)
                    context.DrawPolygon(polygon, Fill, Stroke);
            }
        }

        public static GdPolygonStyle Soft
        {
            get { return new GdPolygonStyle(GdStroke.Soft, GdFill.Soft); }
        }

        public static GdPolygonStyle Sharp
        {
            get { return new GdPolygonStyle(GdStroke.Sharp, GdFill.Sharp); }
        }

        public static GdPolygonStyle Default
        {
            get { return new GdPolygonStyle(GdStroke.Sharp, GdFill.Soft); }
        }
    }
}