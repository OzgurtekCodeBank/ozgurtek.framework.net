using NetTopologySuite.Geometries;
using ozgurtek.framework.core.Mapping;
using System;

namespace ozgurtek.framework.common.Mapping
{
    public class GdViewport : IGdViewport
    {
        private Envelope _world = new Envelope(-20026376.39, 20026376.39, -20048966.10, 20048966.10);
        private int _srid = 3857;
        
        private Envelope _view = new Envelope(double.MinValue, double.MaxValue, double.MinValue, double.MaxValue);

        private double _u = 0.000197916;
        private double _v = 0.000194;

        public EventHandler ViewPortChanged;

        public Envelope World
        {
            get
            {
                return _world;
            }
            set
            {
                _world = value;
                AdjustAspectRatio();
                if (ViewPortChanged != null)
                    ViewPortChanged(this, EventArgs.Empty);
            }
        }

        public Envelope View
        {
            get
            {
                return _view;
            }
            set
            {
                _view = value;
                AdjustAspectRatio();
                if (ViewPortChanged != null)
                    ViewPortChanged(this, EventArgs.Empty);

            }
        }

        public Coordinate WorldtoView(Coordinate coordinate)
        {
            double dx = -_world.MinX;
            double dy = -_world.MinY;
            double sx = View.Width / _world.Width;
            double sy = View.Height / _world.Height;

            double a = View.MinX + (coordinate.X + dx) * sx;
            double b = View.MinY + (View.Height - (coordinate.Y + dy) * sy);

            return new Coordinate(a, b);
        }

        public Coordinate ViewtoWorld(Coordinate coordinate)
        {
            double dx = _world.MinX;
            double dy = _world.MinY;
            double sx = _world.Width / View.Width;
            double sy = _world.Height / View.Height;
            double x = coordinate.X;
            double y = View.Height - coordinate.Y;
            double a = (View.MinX + x) * sx + dx;
            double b = (View.MinY + y) * sy + dy;

            Coordinate res = new Coordinate(a, b);
            return res;
        }

        public int Srid
        {
            get => _srid;
            set => _srid = value;
        }

        public double Scale
        {
            get
            {
                double u = (_view.Width * _u) / _world.Width;
                double v = (_view.Height * _v) / _world.Height;
                return Math.Max(u, v);
            }
            set
            {
                double w = _view.Width / value * _u;
                double h = _view.Height / value * _v;
                Envelope envelope = new Envelope(0, w, 0, h);
                Coordinate centre = _world.Centre;
                envelope.Translate(centre.X, centre.Y);
                World = envelope;
                if (ViewPortChanged != null)
                    ViewPortChanged(this, EventArgs.Empty);
            }
        }

        public double PixelScale
        {
            get
            {
                double a = Math.Sqrt(Math.Pow(_world.Width, 2) + Math.Pow(_world.Height, 2));
                double b = Math.Sqrt(Math.Pow(_view.Width, 2) + Math.Pow(_view.Height, 2));
                return a / b;
            }
        }

        public EventHandler Changed
        {
            get { return ViewPortChanged; }
            set { ViewPortChanged = value; }
        }

        protected void AdjustAspectRatio()
        {
            double newMinX = _world.MinX;
            double newMinY = _world.MinY;
            double newMaxX = _world.MaxX;
            double newMaxY = _world.MaxY;

            double ratio = View.Width / View.Height;
            if (ratio > 1)
            {
                double width = _world.Height * ratio;
                if (width < _world.Width)
                {
                    double height = _world.Width * (1 / ratio);
                    double dy = Math.Abs(height - _world.Height) / 2;
                    newMinY -= dy;
                    newMaxY += dy;
                }
                else
                {
                    double dx = Math.Abs(width - _world.Width) / 2;
                    newMinX -= dx;
                    newMaxX += dx;
                }
            }
            else if (ratio < 1)
            {
                double height = _world.Width * (1 / ratio);
                if (height < _world.Height)
                {
                    double width = _world.Height * ratio;
                    double dx = Math.Abs(width - _world.Width) / 2;
                    newMinX -= dx;
                    newMaxX += dx;
                }
                else
                {
                    double dy = Math.Abs(height - _world.Height) / 2;
                    newMinY -= dy;
                    newMaxY += dy;
                }
            }
            _world = new Envelope(newMinX, newMaxX, newMinY, newMaxY);
        }
    }
}