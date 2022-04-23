using System;
using ozgurtek.framework.common.Mapping;
using ozgurtek.framework.common.Util;
using ozgurtek.framework.core.Mapping;
using ozgurtek.framework.core.Style;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System.Collections.Generic;
using System.Threading;

namespace ozgurtek.framework.ui.map.skiasharp
{
    internal class GdSkLayerRenderer : GdSkAbstractRenderer
    {
        private readonly SKBitmap _backBuffer;
        private readonly SKCanvas _backBufferCanvas;
        private readonly List<GdTrack> _trackList = new List<GdTrack>();

        public GdSkLayerRenderer(GdSkMapInternal map) 
            : base(map)
        {
            SKImageInfo info = new SKImageInfo((int)map.Viewport.View.Width, (int)map.Viewport.View.Height);
            _backBuffer = new SKBitmap(info);
            _backBufferCanvas = new SKCanvas(_backBuffer);

            Dirty = true;
        }

        public SKBitmap BackBuffer
        {
            get { return _backBuffer; }
        }

        public override void Render(SKPaintSurfaceEventArgs e)
        {
            if (Dirty)
            {
                Dirty = false;
                AbortRender();

                GdTrack newTrack = new GdTrack();
                _trackList.Add(newTrack);

                Thread thread = new Thread(RenderThread);
                thread.Start(newTrack);
            }

            e.Surface.Canvas.DrawBitmap(BackBuffer, 0, 0);
        }

        private void RenderThread(object param)
        {
            try
            {
                //GdLicenseManager.Instance.CheckValid();

                //System.Diagnostics.Debug.WriteLine("map render started");
                //System.Diagnostics.Debug.WriteLine(
                //    $"viewport: {Map.Viewport.World.MinX},{Map.Viewport.World.MaxX},{Map.Viewport.World.MinY},{Map.Viewport.World.MaxY},");

                GdTrack track = (GdTrack)param;
                _backBufferCanvas.Clear(new SKColor(Map.BackColor.R, Map.BackColor.G, Map.BackColor.B));
                GdSkRenderContext context = new GdSkRenderContext(_backBufferCanvas, (GdViewport)Map.Viewport, Map.Antialias);

                double mapScale = 1 / Map.Viewport.Scale;
                RenderLayer(track, context, mapScale);
                RenderLabel(track, context, mapScale);

                _trackList.Remove(track);
                //System.Diagnostics.Debug.WriteLine("map render finish");
            }
            catch(Exception e)
            {
                DrawMessageOnMap(_backBufferCanvas, $"{e.Message}");
            }
        }

        private void RenderLayer(GdTrack track, GdSkRenderContext context, double mapScale)
        {
            List<IGdLayer> layers = new List<IGdLayer>(Map.LayerCollection);
            foreach (IGdLayer layer in layers)
            {
                try
                {
                    if (track.CancellationPending)
                        break;

                    if (layer.Renderer == null)
                        continue;

                    if (!IsRenderable(layer.Renderer.Style, mapScale))
                        continue;

                    layer.Renderer.Render(context, track);
                }
                catch(Exception e)
                {
                    DrawMessageOnMap(_backBufferCanvas, $"{layer.Name} {e.Message}");
                }
            }
        }

        private void RenderLabel(GdTrack track, GdSkRenderContext context, double mapScale)
        {
            List<IGdLayer> layers = new List<IGdLayer>(Map.LayerCollection);
            foreach (IGdLayer layer in layers)
            {
                try
                {
                    if (track.CancellationPending)
                        break;

                    if (!(layer is IGdLabeledLayer labeledLayer))
                        continue;

                    if (layer.Renderer == null)
                        continue;

                    if (!IsRenderable(layer.Renderer.Style, mapScale))
                        continue;

                    if (labeledLayer.LabelRenderer == null)
                        continue;

                    if (!IsRenderable(labeledLayer.LabelRenderer.Style, mapScale))
                        continue;

                    labeledLayer.LabelRenderer.Render(context, track);
                }
                catch(Exception e)
                {
                    DrawMessageOnMap(_backBufferCanvas, $"{layer.Name} {e.Message}");
                }
            }
        }

        private bool IsRenderable(IGdStyle style, double mapScale)
        {
            if (style == null || !style.Visible)
                return false;

            double? maxScale = style.MaxScale;
            if (maxScale.HasValue && mapScale <= maxScale.Value)
                return false;

            double? minScale = style.MinScale;
            if (minScale.HasValue && mapScale >= minScale.Value)
                return false;

            return true;
        }

        public override void Dispose()
        {
            AbortRender();
            while (_trackList.Count > 0) { }

            _backBuffer.Dispose();
            _backBufferCanvas.Dispose();
        }

        public void AbortRender()
        {
            foreach (GdTrack oldTrack in _trackList)
                oldTrack.SetCancelationPendingTrue();
        }
    }
}