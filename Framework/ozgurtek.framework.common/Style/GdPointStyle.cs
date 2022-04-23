using NetTopologySuite.Geometries;
using ozgurtek.framework.core.Mapping;
using ozgurtek.framework.core.Style;

namespace ozgurtek.framework.common.Style
{
    public class GdPointStyle : GdAbstractStyle, IGdPointStyle
    {
        private int _size = 4;
        private IGdStroke _stroke = new GdStroke();
        private IGdFill _fill = new GdFill();
        private GdPointStyleType _type = GdPointStyleType.Circle;

        public GdPointStyle()
        {
        }

        public GdPointStyle(GdPointStyleType type, IGdStroke stroke, IGdFill fill)
        {
            _type = type;
            _stroke = stroke;
            _fill = fill;
        }

        public GdPointStyleType PointStleType
        {
            get { return _type; }
            set { _type = value; }
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

        public int Size
        {
            get => _size;
            set => _size = value;
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

                if (thisGeom is Point pt)
                    context.DrawPoint(pt, _type, _size, _stroke, _fill);
            }
        }

        public static GdPointStyle Soft
        {
            get { return new GdPointStyle(GdPointStyleType.Circle, GdStroke.Soft, GdFill.Soft); }
        }

        public static GdPointStyle Sharp
        {
            get { return new GdPointStyle(GdPointStyleType.Circle, GdStroke.Sharp, GdFill.Sharp); }
        }

        public static GdPointStyle Default
        {
            get { return new GdPointStyle(GdPointStyleType.Circle, GdStroke.Sharp, GdFill.Soft); }
        }
    }
}