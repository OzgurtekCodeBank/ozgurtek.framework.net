using SkiaSharp.Views.Forms;
using System;
using SkiaSharp;

namespace ozgurtek.framework.ui.map.skiasharp
{
    internal abstract class GdSkAbstractRenderer : IDisposable
    {
        protected readonly GdSkMapInternal Map;

        protected GdSkAbstractRenderer(GdSkMapInternal map)
        {
            Map = map;
        }

        public bool Dirty;
        public bool Busy;

        public abstract void Render(SKPaintSurfaceEventArgs e);
        public abstract void Dispose();

        public void DrawMessageOnMap(SKCanvas canvas, string message)
        {
            SKPaint rectanglePaint = new SKPaint();

            float width = (float)Map.Viewport.View.Width;
            float height = (float)Map.Viewport.View.Height;
            rectanglePaint.Color = SKColors.WhiteSmoke;
            rectanglePaint.Style = SKPaintStyle.Fill;
            canvas.DrawRect(0, height / 2f - 10, width, 20, rectanglePaint);

            SKPaint textPaint = new SKPaint();
            textPaint.TextSize = 10;
            textPaint.TextAlign = SKTextAlign.Center;
            textPaint.Style = SKPaintStyle.StrokeAndFill;
            canvas.DrawText(message, new SKPoint(width / 2f, height / 2f), textPaint);
            canvas.Flush();
        }
    }
}
