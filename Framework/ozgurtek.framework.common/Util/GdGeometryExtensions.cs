using NetTopologySuite.Geometries;
using NetTopologySuite.Utilities;
using System.Collections.Generic;

namespace ozgurtek.framework.common.Util
{
    public static class GdGeometryExtensions
    {
        public static List<LineSegment> GetLines(this LineString lineString)
        {
            List<LineSegment> segments = new List<LineSegment>();
            int numPoints = lineString.NumPoints;
            for (int i = 0; i < numPoints - 1; i++)
            {
                Point point0 = lineString.GetPointN(i);
                Point point1 = lineString.GetPointN(i + 1);
                segments.Add(new LineSegment(point0.Coordinate, point1.Coordinate));
            }
            return segments;
        }

        public static double AngleInDegrees(this LineSegment lineSegment)
        {
            double angle = Radians.ToDegrees(lineSegment.Angle);
            return (angle % 360 + 360) % 360;
        }
    }
}