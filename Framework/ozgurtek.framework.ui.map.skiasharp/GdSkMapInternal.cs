using NetTopologySuite.Geometries;
using ozgurtek.framework.common.Mapping;
using ozgurtek.framework.core.Data;
using ozgurtek.framework.core.Mapping;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using ozgurtek.framework.ui.map.skiasharp.Touch;
using Xamarin.Forms;

namespace ozgurtek.framework.ui.map.skiasharp
{
    internal class GdSkMapInternal : SKCanvasView, IGdMap
    {
        private bool _antialias = true;
        private GdColor _backColor;
        private IGdViewport _viewport;
        private IGdLayerCollection _layerCollection;

        internal GdSkLayerRenderer LayerRenderer;
        internal GdSkGestureRenderer GestureRenderer;
        internal GdSkMarkerRenderer MarkerRenderer;
        private IGdMapController _controller;

        private bool _allowPanController = true;
        private bool _allowPinchController = true;
        private bool _allowMouseWheelController = true;
        private bool _allowDoubleTappedController = true;
        private IList<IGdMarker> _markers;

        internal GdSkMapInternal()
        {
            _backColor = new GdColor(192, 192, 192);
            _layerCollection = new GdLayerCollection();
            _viewport = new GdViewport();
            _markers = new List<IGdMarker>();

            Touch += SkMapTouch;//to wheel
            EnableTouchEvents = true;
            IgnorePixelScaling = true;
            
            Device.StartTimer(TimeSpan.FromSeconds(1f / 60), () =>
            {
                InvalidateSurface();
                return true;
            });
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            LayerRenderer?.Dispose();
            MarkerRenderer?.Dispose();
            GestureRenderer?.Dispose();

            LayerRenderer = null;
            MarkerRenderer = null;
            GestureRenderer = null;
        }

        //to wheel
        private void SkMapTouch(object sender, SKTouchEventArgs e)
        {
            if (e.ActionType != SKTouchAction.WheelChanged)
                return;
            
            GestureRenderer.OnWheelEvent(e);

            if (Controller != null)
                Controller.OnWheelChanged(new Coordinate(e.Location.X, e.Location.Y), e.WheelDelta);
        }

        public void OnTouchEffectAction(object sender, GdTouchActionEventArgs args)
        {
            Coordinate coordinate = new Coordinate(args.Location.X, args.Location.Y);
            if (Controller != null)
            {
                switch (args.Type)
                {
                    case GdTouchActionType.Entered:
                        Debug.WriteLine($"Entered ID: {args.Id} IsConcat {args.IsInContact}");
                        Controller.OnEntered(coordinate, args.Id, args.IsInContact);
                        break;
                    case GdTouchActionType.Moved:
                        Debug.WriteLine($"Moved ID: {args.Id} IsConcat {args.IsInContact}");
                        Controller.OnMoved(coordinate, args.Id, args.IsInContact);
                        break;
                    case GdTouchActionType.Exited:
                        Debug.WriteLine($"Exited ID: {args.Id} IsConcat {args.IsInContact}");
                        Controller.OnExited(coordinate, args.Id, args.IsInContact);
                        break;
                    case GdTouchActionType.Pressed:
                        Debug.WriteLine($"Pressed ID: {args.Id} IsConcat {args.IsInContact}");
                        Controller.OnPressed(coordinate, args.Id, args.IsInContact);
                        break;
                    case GdTouchActionType.Released:
                        Debug.WriteLine($"Released ID: {args.Id} IsConcat {args.IsInContact}");
                        Controller.OnReleased(coordinate, args.Id, args.IsInContact);
                        break;
                    case GdTouchActionType.Cancelled:
                        Debug.WriteLine($"Cancelled ID: {args.Id} IsConcat {args.IsInContact}");
                        Controller.OnCanceled(coordinate, args.Id, args.IsInContact);
                        break;
                }
            }
            if (GestureRenderer != null)
                GestureRenderer.ProcessTouchEvent(args.Id, args.Type, args.Location);
        }

        protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
        {
            base.OnPaintSurface(e);

            if ((int) _viewport.View.Height != e.Info.Height || (int) _viewport.View.Width != e.Info.Width)
                _viewport.View = new Envelope(0, e.Info.Width, 0, e.Info.Height);

            if (LayerRenderer == null)
                LayerRenderer = new GdSkLayerRenderer(this);

            if (GestureRenderer == null)
                GestureRenderer = new GdSkGestureRenderer(this);

            if (MarkerRenderer == null)
                MarkerRenderer = new GdSkMarkerRenderer(this);

            e.Surface.Canvas.Clear(new SKColor(_backColor.R, _backColor.G, _backColor.B));

            GestureRenderer.Render(e);
            if (GestureRenderer.Busy)
                return;

            LayerRenderer.Render(e);
            MarkerRenderer.Render(e);

            if (_controller != null)
                _controller.Render(new GdSkRenderContext(e.Surface.Canvas, _viewport, _antialias));
        }

        public IGdViewport Viewport
        {
            get { return _viewport; }
            set { _viewport = value; }
        }

        public IGdLayerCollection LayerCollection
        {
            get { return _layerCollection; }
            set { _layerCollection = value; }
        }

        public void Render(bool layer = true, bool marker = true)
        {
            if (LayerRenderer != null)
                LayerRenderer.Dirty = layer;

            if (MarkerRenderer != null)
                MarkerRenderer.Dirty = marker;
        }

        public GdColor BackColor
        {
            get { return _backColor; }
            set { _backColor = value; }
        }

        public void AbortRender()
        {
            if (LayerRenderer != null)
                LayerRenderer.AbortRender();
        }

        public IGdMapController Controller
        {
            get { return _controller; }
            set
            {
                if (_controller != null)
                    _controller.EndInteraction();

                if (value != null)
                    value.StartInteraction(this);

                _controller = value;
            }
        }

        public IList<IGdMarker> Markers
        {
            get { return _markers; }
            set { _markers = value; }
        }

        public bool Antialias
        {
            get => _antialias;
            set => _antialias = value;
        }

        public bool AllowPanController
        {
            get => _allowPanController;
            set => _allowPanController = value;
        }

        public bool AllowPinchController
        {
            get => _allowPinchController;
            set => _allowPinchController = value;
        }

        public bool AllowMouseWheelController
        {
            get => _allowMouseWheelController;
            set => _allowMouseWheelController = value;
        }

        public bool AllowDoubleTappedController
        {
            get => _allowDoubleTappedController;
            set => _allowDoubleTappedController = value;
        }
    }
}