using NetTopologySuite.Geometries;
using ozgurtek.framework.core.Mapping;
using ozgurtek.framework.core.Style;

namespace ozgurtek.framework.common.Style
{
    public class GdLineStyle : GdAbstractStyle, IGdLineStyle
    {
        private IGdStroke _stroke = new GdStroke();

        public GdLineStyle()
        {
        }

        public GdLineStyle(IGdStroke stroke)
        {
            _stroke = stroke;
        }

        public IGdStroke Stroke
        {
            get { return _stroke; }
            set { _stroke = value; }
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

                if (thisGeom is LineString lineString)
                    context.DrawLine(lineString, Stroke);
            }
        }

        public static GdLineStyle Soft
        {
            get { return new GdLineStyle(GdStroke.Soft); }
        }

        public static GdLineStyle Sharp
        {
            get { return new GdLineStyle(GdStroke.Sharp); }
        }

        public static GdLineStyle Default
        {
            get { return new GdLineStyle(GdStroke.Sharp); }
        }
    }
}