using System;
using System.Collections.Generic;
using ozgurtek.framework.common.Mapping;
using ozgurtek.framework.common.Util;
using ozgurtek.framework.core.Mapping;
using SkiaSharp;
using SkiaSharp.Views.Forms;

namespace ozgurtek.framework.ui.map.skiasharp
{
    internal class GdSkMarkerRenderer : GdSkAbstractRenderer
    {
        public GdSkMarkerRenderer(GdSkMapInternal map) 
            : base(map)
        {
            Dirty = true;
        }

        public override void Render(SKPaintSurfaceEventArgs e)
        {
            GdTrack newTrack = new GdTrack();
            object[] objects = {newTrack, e};
            RenderThread(objects);
        }

        public override void Dispose()
        {
        }

        private void RenderThread(object param)
        {
            //System.Diagnostics.Debug.WriteLine("marker render started");
            //System.Diagnostics.Debug.WriteLine(
            //    $"viewport: {Map.Viewport.World.MinX},{Map.Viewport.World.MaxX},{Map.Viewport.World.MinY},{Map.Viewport.World.MaxY},");

            object[] objects = (object[])param;
            GdTrack track = (GdTrack)objects[0];
            SKPaintSurfaceEventArgs args = (SKPaintSurfaceEventArgs)objects[1];
            GdSkRenderContext context = new GdSkRenderContext(args.Surface.Canvas, (GdViewport)Map.Viewport, Map.Antialias);
            RenderMarker(track, context, args.Surface.Canvas);

            //System.Diagnostics.Debug.WriteLine("marker render finish");
        }

        private void RenderMarker(GdTrack track, GdSkRenderContext context, SKCanvas surfaceCanvas)
        {
            try
            {
                List<IGdMarker> markers = new List<IGdMarker>(Map.Markers);
                foreach (IGdMarker marker in markers)
                {
                    marker.Render(context, track);
                }
            }
            catch (Exception e)
            {
                DrawMessageOnMap(surfaceCanvas, $"{e.Message}");
            }
        }
    }
}
