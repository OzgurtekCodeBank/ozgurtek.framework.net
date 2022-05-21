using System.IO;
using System.Reflection;
using NetTopologySuite.Geometries;
using ozgurtek.framework.core.Data;
using ozgurtek.framework.core.Mapping;
using ozgurtek.framework.test.xamarin.Managers;
using SkiaSharp;
using Xamarin.Forms;
using Point = NetTopologySuite.Geometries.Point;

namespace ozgurtek.framework.test.xamarin.Pages.Map
{
    public class ImageMarker : IGdMarker
    {
        private readonly Point _point;
        private SKBitmap _bitmap;
        private int _size;
        private int _rotate;
        private bool _disposed;

        public ImageMarker(Point point, string source)
        {
            _size = Device.RuntimePlatform == Device.UWP ? 20 : 7;
            _point = point;
            CreateImage(source);
        }

        private void CreateImage(string source)
        {
            Assembly assembly = GetType().GetTypeInfo().Assembly;
            source = "ozgurtek.framework.test.xamarin.Images." + source;
            using (Stream stream = assembly.GetManifestResourceStream(source))
            {
                _bitmap = SKBitmap.Decode(stream);
            }
        }

        public int Size
        {
            get => _size;
            set => _size = value;
        }

        public int Rotate
        {
            get => _rotate;
            set => _rotate = value;
        }

        public void Render(IGdRenderContext context, IGdTrack track = null)
        {
            if (_disposed)
                return;

            try
            {
                SKCanvas canvas = (SKCanvas)context.NativeObject;
                Coordinate viewCoord = context.Viewport.WorldtoView(_point.Coordinate);
                Polygon polygon = GdApp.Instance.Util.CreatePolygonFromPixel(viewCoord, Size);
                Envelope envelope = polygon.EnvelopeInternal;
                SKRect rect = new SKRect((float)envelope.MinX, (float)envelope.MinY, (float)envelope.MaxX,
                    (float)envelope.MaxY);

                if (_disposed)
                    return;

                canvas.DrawBitmap(_bitmap, rect);
            }
            catch
            {
                // ignored
            }
        }

        public void Dispose()
        {
            _disposed = true;
            _bitmap.Dispose();
        }
    }
}
