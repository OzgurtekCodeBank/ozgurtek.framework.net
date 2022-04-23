using NetTopologySuite.Geometries;
using ozgurtek.framework.core.Style;

namespace ozgurtek.framework.core.Mapping
{
    public interface IGdRenderContext
    {
        IGdViewport Viewport { get; }

        void DrawLine(LineString line, IGdStroke stroke);

        void DrawPolygon(Polygon polygon, IGdFill fill, IGdStroke stroke);

        void DrawPoint(Point point, GdPointStyleType type, int size, IGdStroke stroke, IGdFill fill);

        void DrawImage(Polygon polygon, byte[] image, double transparent, IGdStroke stroke);

        void DrawText(Point point, string text, int size, double rotation, IGdStroke stroke, IGdFill fill);

        void Flush();

        object NativeObject { get; }
    }
}
