using NetTopologySuite.Geometries;
using ozgurtek.framework.core.Mapping;
using ozgurtek.framework.core.Style;

namespace ozgurtek.framework.common.Style
{
    public class GdImageStyle : GdAbstractStyle, IGdImageStyle
    {
        private double _transparent;
        private IGdStroke _stroke;

        public override void Render(IGdRenderContext context, Geometry geometry, object options = null)
        {
            if (!(options is byte[] image))
                return;

            if (geometry == null)
                return;

            int numGeometries = geometry.NumGeometries;
            for (int i = 0; i < numGeometries; i++)
            {
                Geometry thisGeom = geometry.GetGeometryN(i);
                if (thisGeom.NumGeometries > 1)
                    Render(context, thisGeom, options);

                if (thisGeom is Polygon polygon)
                    context.DrawImage(polygon, image, _transparent, _stroke);
            }
        }

        public IGdStroke Stroke
        {
            get { return _stroke; }
            set { _stroke = value; }
        }

        public double Transparent
        {
            get => _transparent;
            set => _transparent = value;
        }
    }
}