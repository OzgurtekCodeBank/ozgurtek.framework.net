using NetTopologySuite.Geometries;
using ozgurtek.framework.core.Mapping;
using ozgurtek.framework.core.Style;

namespace ozgurtek.framework.common.Style
{
    public abstract class GdAbstractStyle : IGdStyle
    {
        private bool _visible = true;

        public bool Visible
        {
            get => _visible;
            set => _visible = value;
        }

        public double? MinScale { get; set; }

        public double? MaxScale { get; set; }

        public abstract void Render(IGdRenderContext context, Geometry geometry, object options = null);
    }
}
