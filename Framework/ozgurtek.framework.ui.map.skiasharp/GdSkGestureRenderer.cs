using System;
using NetTopologySuite.Geometries;
using ozgurtek.framework.common.Mapping;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System.Collections.Generic;
using System.Timers;
using ozgurtek.framework.ui.map.skiasharp.Touch;
using Xamarin.Forms;
using Point = Xamarin.Forms.Point;

namespace ozgurtek.framework.ui.map.skiasharp
{
    internal class GdSkGestureRenderer : GdSkAbstractRenderer
    {
        private SKMatrix Matrix { get; set; }
        private readonly GdSkMapInternal _map;
        private readonly Timer _wheelTimer = new Timer(500);
        private readonly Dictionary<long, GdTouchManipulationInfo> _touchDictionary = new Dictionary<long, GdTouchManipulationInfo>();

        public GdSkGestureRenderer(GdSkMapInternal map) : base(map)
        {
            _map = map;
            Matrix = SKMatrix.CreateIdentity();
            _wheelTimer.Elapsed += WheelTimer_Elapsed;
            _wheelTimer.AutoReset = false;
        }

        internal void ProcessTouchEvent(long id, GdTouchActionType type, Point pt)
        {
            SKPoint location =
                new SKPoint((float)(_map.Viewport.View.Width * pt.X / _map.Viewport.View.Width),
                    (float)(_map.Viewport.View.Height * pt.Y / _map.Viewport.View.Height));

            switch (type)
            {
                case GdTouchActionType.Pressed:
                    Busy = true;
                    if (!_touchDictionary.ContainsKey(id))
                        _touchDictionary.Add(id, new GdTouchManipulationInfo
                        {
                            PreviousPoint = location,
                            NewPoint = location
                        });
                    break;

                case GdTouchActionType.Moved:
                    if (_touchDictionary.ContainsKey(id))
                    {
                        _touchDictionary[id].NewPoint = location;
                        Manipulate();
                        _touchDictionary[id].PreviousPoint = _touchDictionary[id].NewPoint;
                    }
                    break;

                case GdTouchActionType.Released:
                    if (_touchDictionary.ContainsKey(id))
                    {
                        _touchDictionary[id].NewPoint = location;
                        Manipulate();

                        _touchDictionary.Remove(id);
                        if (_touchDictionary.Count == 0)
                            Busy = false;
                        Finish();
                    }
                    break;
            }
        }

        internal void Manipulate()
        {
            GdTouchManipulationInfo[] infos = new GdTouchManipulationInfo[_touchDictionary.Count];
            _touchDictionary.Values.CopyTo(infos, 0);
            SKMatrix touchMatrix = SKMatrix.CreateIdentity();

            if (infos.Length == 1)
            {
                SKPoint prevPoint = infos[0].PreviousPoint;
                SKPoint newPoint = infos[0].NewPoint;
                SKPoint pivotPoint = Matrix.MapPoint((float)(_map.Viewport.View.Width / 2), (float)(_map.Viewport.View.Height / 2));
                touchMatrix = OneFingerManipulate(prevPoint, newPoint, pivotPoint);
            } 
            else if (infos.Length >= 2)
            {
                int pivotIndex = infos[0].NewPoint == infos[0].PreviousPoint ? 0 : 1;
                SKPoint pivotPoint = infos[pivotIndex].NewPoint;
                SKPoint newPoint = infos[1 - pivotIndex].NewPoint;
                SKPoint prevPoint = infos[1 - pivotIndex].PreviousPoint;

                touchMatrix = TwoFingerManipulate(prevPoint, newPoint, pivotPoint);
            }

            SKMatrix matrix = Matrix;
            matrix = matrix.PostConcat(touchMatrix);

            Matrix = matrix;
        }

        internal SKMatrix OneFingerManipulate(SKPoint prevPoint, SKPoint newPoint, SKPoint pivotPoint)
        {
            SKPoint delta = newPoint - prevPoint;
            SKMatrix matrix = SKMatrix.CreateIdentity();

            if (Device.RuntimePlatform == Device.macOS)
                delta.Y = -delta.Y;
            matrix = matrix.PostConcat(SKMatrix.CreateTranslation(delta.X, delta.Y));
            return matrix;
        }

        internal SKMatrix TwoFingerManipulate(SKPoint prevPoint, SKPoint newPoint, SKPoint pivotPoint)
        {
            SKMatrix touchMatrix = SKMatrix.CreateIdentity();
            SKPoint oldVector = prevPoint - pivotPoint;
            SKPoint newVector = newPoint - pivotPoint;

            float scaleX = newVector.X / oldVector.X;
            float scaleY = newVector.Y / oldVector.Y;
            
            float pivotX = (pivotPoint.X + newPoint.X) / 2;
            float pivotY = (pivotPoint.Y + newPoint.Y) / 2;
            float scale = Magnitude(newVector) / Magnitude(oldVector);

            if (!float.IsNaN(scaleX) && !float.IsInfinity(scaleX) && !float.IsNaN(scaleY) && !float.IsInfinity(scaleY))
            {
                touchMatrix = touchMatrix.PostConcat(SKMatrix.CreateScale(scale, scale, pivotX, pivotY));
            }

            return touchMatrix;
        }

        internal float Magnitude(SKPoint point)
        {
            return (float)Math.Sqrt(Math.Pow(point.X, 2) + Math.Pow(point.Y, 2));
        }
        
        internal void OnWheelEvent(SKTouchEventArgs skTouchEventArgs)
        {
            if (_map.AllowMouseWheelController)
            {
                _wheelTimer.Stop();
                WheelManipulate(skTouchEventArgs);
                _wheelTimer.Start();
            }
        }

        private void WheelManipulate(SKTouchEventArgs args)
        {
            SKMatrix touchMatrix = SKMatrix.CreateIdentity();
            int wheelDelta = args.WheelDelta;
            Busy = true;
            if (wheelDelta != 0)
            {
                SKPoint pivotPoint = args.Location;
                touchMatrix = WheelManipulate(wheelDelta, pivotPoint);
            }
            SKMatrix matrix = Matrix;
            matrix = matrix.PostConcat(touchMatrix);

            Matrix = matrix;
        }
        
        private SKMatrix WheelManipulate(int wheelDelta, SKPoint pivotPoint)
        {
            float scaleX;
            float scaleY;
            float delta = wheelDelta > 0 ? 0.3f : -0.3f;
            scaleX = scaleY = 1f + delta;
            SKMatrix scaleMatrix = SKMatrix.CreateScale(scaleX, scaleY, pivotPoint.X, pivotPoint.Y);
            return scaleMatrix;
        }

        public override void Render(SKPaintSurfaceEventArgs e)
        {
            e.Surface.Canvas.Save();
            SKMatrix matrix = Matrix;
            e.Surface.Canvas.Concat(ref matrix);
            e.Surface.Canvas.DrawBitmap(_map.LayerRenderer.BackBuffer, 0, 0);
            e.Surface.Canvas.Restore();
        }

        private void Finish()
        {
            if (!Matrix.IsIdentity)
            {
                ReCalculateViewPort(Matrix);
                _map.Render();
            }
            Matrix = SKMatrix.CreateIdentity();
            Busy = false;
        }

        private void WheelTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Finish();
        }

        private void ReCalculateViewPort(SKMatrix matrix)
        {
            GdViewport viewport = (GdViewport)_map.Viewport;
            if (matrix.IsIdentity)
                return;

            SKMatrix invert = matrix.Invert();
            Envelope view = viewport.View;
            SKPoint pointMin = invert.MapPoint((float)view.MinX, (float)view.MinY);
            SKPoint pointMax = invert.MapPoint((float)view.MaxX, (float)view.MaxY);
            Coordinate c1 = viewport.ViewtoWorld(new Coordinate(pointMin.X, pointMin.Y));
            Coordinate c2 = viewport.ViewtoWorld(new Coordinate(pointMax.X, pointMax.Y));
            Envelope env = new Envelope(c1.X, c2.X, c1.Y, c2.Y);
            viewport.World = env;
        }

        public override void Dispose()
        {
        }
    }
}