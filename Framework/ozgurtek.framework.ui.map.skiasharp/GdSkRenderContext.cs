using NetTopologySuite.Geometries;
using ozgurtek.framework.common.Geodesy;
using ozgurtek.framework.core.Data;
using ozgurtek.framework.core.Mapping;
using ozgurtek.framework.core.Style;
using SkiaSharp;
using System;
using System.Collections.Generic;

namespace ozgurtek.framework.ui.map.skiasharp
{
    internal class GdSkRenderContext : IGdRenderContext
    {
        private readonly SKCanvas _canvas;
        private IGdViewport _viewport;
        private readonly bool _antialias;

        public GdSkRenderContext(SKCanvas canvas, IGdViewport viewport, bool antialias)
        {
            _canvas = canvas;
            _viewport = viewport;
            _antialias = antialias;
        }

        public IGdViewport Viewport
        {
            get => _viewport;
            set => _viewport = value;
        }

        public void DrawLine(LineString line, IGdStroke stroke)
        {
            if (line.SRID != _viewport.Srid)
                line = (LineString)GdProjection.Project(line, _viewport.Srid);

            if (stroke != null && stroke.Color.A > 0 && stroke.Width > 0)
            {
                SKPath path = CreatePath(line.Coordinates);
                SKPaint paint = CreatePaint(stroke);
                _canvas.DrawPath(path, paint);
                paint.Dispose();
                path.Dispose();
            }
        }

        public void DrawPolygon(Polygon polygon, IGdFill fill, IGdStroke stroke)
        {
            if (polygon.SRID != _viewport.Srid)
                polygon = (Polygon)GdProjection.Project(polygon, _viewport.Srid);

            SKPath path = CreatePath(polygon.ExteriorRing.Coordinates);
            path.Close();

            if (fill != null && fill.Color.A > 0)
            {
                SKPaint paint = CreatePaint(fill);
                _canvas.DrawPath(path, paint);
                paint.Dispose();
            }

            if (stroke != null && stroke.Color.A > 0 && stroke.Width > 0)
            {
                SKPaint paint = CreatePaint(stroke);
                _canvas.DrawPath(path, paint);
                paint.Dispose();
            }

            path.Dispose();
        }

        public void DrawPoint(Point point, GdPointStyleType type, int size, IGdStroke stroke, IGdFill fill)
        {
            if (point.SRID != _viewport.Srid)
                point = (Point)GdProjection.Project(point, _viewport.Srid);

            SKPoint pt = CreatePoint(point.Coordinate);
            float half = Convert.ToSingle(size / 2);
            pt = new SKPoint(pt.X + half, pt.Y + half);

            if (fill != null && fill.Color.A > 0)
            {
                SKPaint paint = CreatePaint(fill);
                if (type == GdPointStyleType.Circle)
                {
                    _canvas.DrawCircle(pt, size, paint);
                }
                else if (type == GdPointStyleType.Square)
                {
                    _canvas.DrawRect(pt.X, pt.Y, size, size, paint);
                }
                paint.Dispose();
            }

            if (stroke != null && stroke.Color.A > 0 && stroke.Width > 0)
            {
                SKPaint paint = CreatePaint(stroke);
                if (type == GdPointStyleType.Circle)
                {
                    _canvas.DrawCircle(pt, size, paint);
                }
                else if (type == GdPointStyleType.Square)
                {
                    _canvas.DrawRect(pt.X, pt.Y, size, size, paint);
                }
                paint.Dispose();
            }
        }

        public void DrawImage(Polygon polygon, byte[] image, double transparent, IGdStroke stroke)
        {
            if (polygon.SRID != _viewport.Srid)
                polygon = (Polygon)GdProjection.Project(polygon, _viewport.Srid);

            SKRect rect = PolygonToSkRect(polygon);
            //System.Diagnostics.Debug.WriteLine($"{rect.Width}-{rect.Height}");
            SKBitmap bitmap = SKBitmap.Decode(image);
            _canvas.DrawBitmap(bitmap, rect);
            bitmap.Dispose();

            if (stroke != null && stroke.Color.A > 0 && stroke.Width > 0)
                DrawPolygon(polygon, null, stroke);
        }

        public void DrawText(Point point, string text, int size, double rotation, IGdStroke stroke, IGdFill fill)
        {
            if (point.SRID != _viewport.Srid)
                point = (Point)GdProjection.Project(point, _viewport.Srid);

            SKPoint pt = CreatePoint(point.Coordinate);

            if (fill != null && fill.Color.A > 0)
            {
                SKPaint paint = CreatePaint(fill);
                paint.TextSize = size;
                paint.TextAlign = SKTextAlign.Center;
                _canvas.Save();
                _canvas.RotateDegrees(-(float)rotation, pt.X, pt.Y);
                _canvas.DrawText(text, pt, paint);
                _canvas.Restore();
                paint.Dispose();
            }

            if (stroke != null && stroke.Color.A > 0 && stroke.Width > 0)
            {
                SKPaint paint = CreatePaint(stroke);
                paint.TextSize = size;
                paint.TextAlign = SKTextAlign.Center;
                _canvas.Save();
                _canvas.RotateDegrees(-(float)rotation, pt.X, pt.Y);
                _canvas.DrawText(text, pt, paint);
                _canvas.Restore();
                paint.Dispose();
            }
        }

        public void Flush()
        {
            _canvas.Flush();
        }

        public object NativeObject
        {
            get
            {
                return _canvas;
            }
        }

        private SKRect PolygonToSkRect(Polygon polygon)
        {
            Geometry boundary = polygon.Boundary;
            Coordinate c1 = boundary.Coordinates[0];
            Coordinate c2 = boundary.Coordinates[2];
            Envelope env = new Envelope(c1, c2);
            Coordinate tl = new Coordinate(env.MinX, env.MaxY);
            Coordinate br = new Coordinate(env.MaxX, env.MinY);
            Coordinate sTl = _viewport.WorldtoView(tl);
            Coordinate sBr = _viewport.WorldtoView(br);

            return new SKRect((float)sTl.X, (float)sTl.Y, (float)sBr.X, (float)sBr.Y);
        }

        private SKPath CreatePath(Coordinate[] coordinates)
        {
            SKPath path = new SKPath();

            List<SKPoint> points = CreatePoints(coordinates);
            path.MoveTo(points[0]);
            for (int i = 1; i < points.Count; i++)
                path.LineTo(points[i]);
            //path.Close();

            return path;
        }

        private List<SKPoint> CreatePoints(Coordinate[] coordinates)
        {
            List<SKPoint> result = new List<SKPoint>(coordinates.Length);
            foreach (Coordinate coordinate in coordinates)
                result.Add(CreatePoint(coordinate));
            return result;
        }

        private SKPoint CreatePoint(Coordinate coordinate)
        {
            Coordinate coor = _viewport.WorldtoView(coordinate);
            return new SKPoint((float)coor.X, (float)coor.Y);
        }

        private SKPaint CreatePaint(IGdFill fill)
        {
            SKPaint paint = new SKPaint();
            paint.IsAntialias = _antialias;
            paint.Style = SKPaintStyle.Fill;
            paint.Color = CreateColor(fill.Color);
            return paint;
        }

        private SKPaint CreatePaint(IGdStroke stroke)
        {
            SKPaint paint = new SKPaint();
            paint.IsAntialias = _antialias;
            paint.Style = SKPaintStyle.Stroke;
            paint.Color = CreateColor(stroke.Color);
            paint.StrokeWidth = stroke.Width;
            return paint;
        }

        private SKColor CreateColor(GdColor color)
        {
            return new SKColor(color.R, color.G, color.B, color.A);
        }
    }
}
