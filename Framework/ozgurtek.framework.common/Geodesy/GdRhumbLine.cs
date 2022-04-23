using System;

namespace ozgurtek.framework.common.Geodesy
{
    public class GdRhumbLine
    {
        private readonly GdLonLat _lonlat;
        private readonly double _bearing;

        public GdRhumbLine(GdLonLat lonlat, double bearing)
        {
            _lonlat = lonlat;
            _bearing = bearing;
        }

        public GdLonLat DestinationPointTo(double distance)
        {
            return _lonlat.DestinationPointTo(distance, _bearing);
        }

        public GdLonLat RhumbDestinationPoint(double distance)
        {
            return _lonlat.RhumbDestinationPoint(distance, _bearing);
        }

        public GdLonLat InterSection(GdLonLat lonLat, double bearing)
        {
            double a1 = RadDegConvert.DegreesToRadians(_lonlat.Lat.Value);
            double a2 = RadDegConvert.DegreesToRadians(lonLat.Lat.Value);
            double b1 = RadDegConvert.DegreesToRadians(_lonlat.Lon.Value);
            double b2 = RadDegConvert.DegreesToRadians(lonLat.Lat.Value);
            double c13 = RadDegConvert.DegreesToRadians(_bearing);
            double c23 = RadDegConvert.DegreesToRadians(bearing);
            double deltaA = a2 - a1;
            double deltaB = b2 - b1;
            double delta12 = 2 * Math.Asin(Math.Sqrt(Math.Sin(deltaA / 2) * Math.Sin(deltaA / 2)
                    + Math.Cos(a1) * Math.Cos(a2) * Math.Sin(deltaB / 2) * Math.Sin(deltaB / 2)));

            if (delta12 == 0)
                return null;

            double ca = Math.Acos((Math.Sin(a2) - Math.Sin(a1) * Math.Cos(delta12)) / (Math.Sin(delta12) * Math.Cos(a1)));

            if (Double.IsNaN(ca)) ca = 0; // protect against rounding

            double vb = Math.Acos((Math.Sin(a1) - Math.Sin(a2) * Math.Cos(delta12)) / (Math.Sin(delta12) * Math.Cos(a2)));
            double c12 = Math.Sin(b2 - b1) > 0 ? ca : 2 * Math.PI - ca;
            double c21 = Math.Sin(b2 - b1) > 0 ? 2 * Math.PI - vb : vb;
            double j1 = c13 - c12; // angle 2-1-3
            double j2 = c21 - c23; // angle 1-2-3

            if (Math.Sin(j1) == 0 && Math.Sin(j2) == 0)
                return null; // infinite intersections

            if (Math.Sin(j1) * Math.Sin(j2) < 0)
                return null;      // ambiguous intersection

            double j3 = Math.Acos(-Math.Cos(j1) * Math.Cos(j2) + Math.Sin(j1) * Math.Sin(j2) * Math.Cos(delta12));
            double delta13 = Math.Atan2(Math.Sin(delta12) * Math.Sin(j1) * Math.Sin(j2), Math.Cos(j2) + Math.Cos(j1) * Math.Cos(j3));
            double a3 = Math.Asin(Math.Sin(a1) * Math.Cos(delta13) + Math.Cos(a1) * Math.Sin(delta13) * Math.Cos(c13));
            double deltaB13 = Math.Atan2(Math.Sin(c13) * Math.Sin(delta13) * Math.Cos(a1), Math.Cos(delta13) - Math.Sin(a1) * Math.Sin(a3));
            double b3 = b1 + deltaB13;
            double latValue = RadDegConvert.RadiansToDegrees(a3);
            double lonValue = ((RadDegConvert.RadiansToDegrees(b3) + 540) % 360) - 180;

            return new GdLonLat()
            {
                Lon = new GdDegree(lonValue),
                Lat = new GdDegree(latValue)
            };
        }
    }
}
