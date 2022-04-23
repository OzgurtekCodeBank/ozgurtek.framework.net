using NetTopologySuite.Geometries;
using ozgurtek.framework.core.Mapping;

namespace ozgurtek.framework.core.Style
{
    public interface IGdStyle
    {
        bool Visible { get; set; }

        double? MinScale { get; set; }

        double? MaxScale { get; set; }

        void Render(IGdRenderContext context, Geometry geometry, object options = null);
    }
}
