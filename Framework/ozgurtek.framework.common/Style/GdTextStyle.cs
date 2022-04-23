using NetTopologySuite.Geometries;
using ozgurtek.framework.common.Util;
using ozgurtek.framework.core.Mapping;
using ozgurtek.framework.core.Style;
using System;
using System.Collections.Generic;

namespace ozgurtek.framework.common.Style
{
    public class GdTextStyle : GdAbstractStyle, IGdTextStyle
    {
        private int _size = 12;
        private IGdStroke _stroke = new GdStroke();
        private IGdFill _fill = new GdFill();

        public GdTextStyle()
        {
        }

        public GdTextStyle(int size, IGdStroke stroke, IGdFill fill)
        {
            _size = size;
            _stroke = stroke;
            _fill = fill;
        }

        public IGdStroke Stroke
        {
            get { return _stroke; }
            set { _stroke = value; }
        }

        public IGdFill Fill
        {
            get { return _fill; }
            set { _fill = value; }
        }

        public int Size
        {
            get => _size;
            set => _size = value;
        }

        public override void Render(IGdRenderContext context, Geometry geometry, object options = null)
        {
            if (!(options is string text))
                return;

            if (geometry == null)
                return;

            int numGeometries = geometry.NumGeometries;
            for (int i = 0; i < numGeometries; i++)
            {
                Geometry thisGeom = geometry.GetGeometryN(i);
                if (thisGeom.NumGeometries > 1)
                    Render(context, thisGeom, options);

                if (thisGeom is LineString lineString) //line string...
                {
                    RenderLineSegment(context, lineString, text);
                }
                else //no line string...
                {
                    Point centroid = thisGeom.Centroid;
                    context.DrawText(centroid, text, _size, 0.0, _stroke, _fill);
                }
            }
        }

        private void RenderLineSegment(IGdRenderContext context, LineString lineString, string text)
        {
            List<LineSegment> lines = lineString.GetLines();
            if (lines.Count == 0)
                return;

            //find mid segment
            int midSegment = (int)Math.Round(Math.Floor(lines.Count / 2.0));
            LineSegment lineSegment = lines[midSegment];

            //lineSegment.PointAlongOffset(0.5, )
            Point point = new Point(lineSegment.MidPoint);
            point.SRID = lineString.SRID;

            double degrees = lineSegment.AngleInDegrees();
            context.DrawText(point, text, _size, degrees, _stroke, _fill);
        }

        public static GdTextStyle Soft
        {
            get { return new GdTextStyle(12, GdStroke.Soft, GdFill.Soft); }
        }

        public static GdTextStyle Sharp
        {
            get { return new GdTextStyle(12, GdStroke.Sharp, GdFill.Sharp); }
        }

        public static GdTextStyle Default
        {
            get { return new GdTextStyle(12, GdStroke.Sharp, GdFill.Soft); }
        }
    }
}