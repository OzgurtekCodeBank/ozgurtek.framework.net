using System;

namespace ozgurtek.framework.common.Geodesy
{
    public class GdLonLat
    {
        public GdDegree Lon { get; set; }

        public GdDegree Lat { get; set; }

        public GdDistance DistanceTo(GdLonLat destinationLonLat)
        {
            double a1 = RadDegConvert.DegreesToRadians(Lat.Value);
            double a2 = RadDegConvert.DegreesToRadians(destinationLonLat.Lat.Value);
            double b1 = RadDegConvert.DegreesToRadians(Lon.Value);
            double b2 = RadDegConvert.DegreesToRadians(destinationLonLat.Lon.Value);
            double delta_a = a2 - a1;
            double delta_b = b2 - b1;
            double A_1 = Math.Sin(delta_a / 2) * Math.Sin(delta_a / 2);
            double A_2 = Math.Cos(a1) * Math.Cos(a2) * Math.Sin(delta_b / 2) * Math.Sin(delta_b / 2);
            double A = A_1 + A_2;
            double C = 2 * Math.Atan2(Math.Sqrt(A), Math.Sqrt(1 - A));
            double D = Constants.RADIUS * C;
            return new GdDistance(D);
        }

        public GdLonLat MidPointTo(GdLonLat destinationLonLat)
        {
            double a1 = RadDegConvert.DegreesToRadians(Lat.Value);
            double a2 = RadDegConvert.DegreesToRadians(destinationLonLat.Lat.Value);
            double b1 = RadDegConvert.DegreesToRadians(Lon.Value);
            double delta_b = RadDegConvert.DegreesToRadians(destinationLonLat.Lon.Value - Lon.Value);
            double bX = Math.Cos(a2) * Math.Cos(delta_b);
            double bY = Math.Sin(a2) * Math.Sin(delta_b);
            double x = Math.Sqrt((Math.Cos(a1) + bX) * (Math.Cos(a1) + bX) + bY * bY);
            double y = Math.Sin(a1) + Math.Sin(a2);
            double a3 = Math.Atan2(y, x);
            double b2 = b1 + Math.Atan2(bY, Math.Cos(a1) + bX);
            double midPointLatValue = RadDegConvert.RadiansToDegrees(a3);
            double midPointLonValue = ((RadDegConvert.RadiansToDegrees(b2) + 540) % 360) - 180;
            return new GdLonLat()
            {
                Lon = new GdDegree(midPointLonValue),
                Lat = new GdDegree(midPointLatValue)
            };
        }

        public GdLonLat IntermediatePointTo(GdLonLat destinationLonLat, double fraction)
        {
            double a1 = RadDegConvert.DegreesToRadians(Lat.Value);
            double a2 = RadDegConvert.DegreesToRadians(destinationLonLat.Lat.Value);
            double b1 = RadDegConvert.DegreesToRadians(Lon.Value);
            double b2 = RadDegConvert.DegreesToRadians(destinationLonLat.Lon.Value);
            double delta_a = a2 - a1;
            double delta_b = b2 - b1;
            double _a = Math.Sin(delta_a / 2) * Math.Sin(delta_a / 2) + Math.Cos(a1) * Math.Cos(a2) * Math.Pow(Math.Sin(delta_b / 2), 2);
            double u = 2 * Math.Atan2(Math.Sqrt(_a), Math.Sqrt(1 - _a));
            double A = Math.Sin((1 - fraction) * u) / Math.Sin(u);
            double B = Math.Sin(fraction * u) / Math.Sin(u);
            double x = A * Math.Cos(a1) * Math.Cos(b1) + B * Math.Cos(a2) * Math.Cos(b2);
            double y = A * Math.Cos(a1) * Math.Sin(b1) + B * Math.Cos(a2) * Math.Sin(b2);
            double z = A * Math.Sin(a1) + B * Math.Sin(a2);
            double a3 = Math.Atan2(z, Math.Sqrt(x * x + y * y));
            double b3 = Math.Atan2(y, x);
            double midPointLatValue = RadDegConvert.RadiansToDegrees(a3);
            double midPointLonValue = ((RadDegConvert.RadiansToDegrees(b3) + 540) % 360) - 180;
            return new GdLonLat()
            {
                Lon = new GdDegree(midPointLonValue),
                Lat = new GdDegree(midPointLatValue)
            };
        }

        public GdLonLat DestinationPointTo(double distance, double bearing)
        {
            double u = distance / (Constants.RADIUS);
            double v = RadDegConvert.DegreesToRadians(bearing);
            double a1 = RadDegConvert.DegreesToRadians(Lat.Value);
            double b1 = RadDegConvert.DegreesToRadians(Lon.Value);
            double sin_a2 = Math.Sin(a1) * Math.Cos(u) + Math.Cos(a1) * Math.Sin(u) * Math.Cos(v);
            double a2 = Math.Asin(sin_a2);
            double y = Math.Sin(v) * Math.Sin(u) * Math.Cos(a1);
            double x = Math.Cos(u) - Math.Sin(a1) * Math.Sin(a2);
            double b2 = b1 + Math.Atan2(y, x);
            double latValue = RadDegConvert.RadiansToDegrees(a2);
            double lonValue = ((RadDegConvert.RadiansToDegrees(b2) + 540) % 360) - 180;
            return new GdLonLat()
            {
                Lon = new GdDegree(lonValue),
                Lat = new GdDegree(latValue)
            };
        }

        public GdDistance CrossTrackDistanceTo(GdLonLat pathStart, GdLonLat pathEnd)
        {
            //double u13 = pathStart.DistanceTo(this).Convert(GdDistanceUnit.M) / Constants.RADIUS;
            //double v13 = RadDegConvert.DegreesToRadians(pathStart.BearingTo(this).Value);
            //double v12 = RadDegConvert.DegreesToRadians(pathStart.BearingTo(pathEnd).Value);
            //double uxt = Math.Asin(Math.Sin(u13) * Math.Sin(v13 - v12));
            //return new GdDistance(uxt * Constants.RADIUS);
            return null;
        }

        public GdDistance AlongTrackDistanceTo(GdLonLat pathStart, GdLonLat pathEnd)
        {
            //double u13 = pathStart.DistanceTo(this).Convert(GdDistanceUnit.M) / Constants.RADIUS;
            //double v13 = RadDegConvert.DegreesToRadians(pathStart.BearingTo(this).Value);
            //double v12 = RadDegConvert.DegreesToRadians(pathStart.BearingTo(pathEnd).Value);
            //double uxt = Math.Asin(Math.Sin(u13) * Math.Sin(v13 - v12));
            //double uat = Math.Acos(Math.Cos(u13) / Math.Abs(Math.Cos(uxt)));
            //return new GdDistance(uat * Math.Sign(Math.Cos(v12 - v13)) * Constants.RADIUS);
            return null;
        }

        public GdBearing BearingTo(GdLonLat lonlat)
        {
            double a1 = RadDegConvert.DegreesToRadians(Lat.Value);
            double a2 = RadDegConvert.DegreesToRadians(lonlat.Lat.Value);
            double delta_b = RadDegConvert.DegreesToRadians(lonlat.Lon.Value - Lon.Value);
            double y = Math.Sin(delta_b) * Math.Cos(a2);
            double x = Math.Cos(a1) * Math.Sin(a2) - Math.Sin(a1) * Math.Cos(a2) * Math.Cos(delta_b);
            double teta = Math.Atan2(y, x);
            double result = RadDegConvert.RadiansToDegrees(teta) + 360;
            return new GdBearing(result % 360);
        }

        public GdBearing FinalBearingTo(GdLonLat lonlat)
        {
            double result = (lonlat.BearingTo(this).Value + 180) % 360;
            return new GdBearing(result);
        }

        public GdDegree MaxLatitude(double bearing)
        {
            double u = RadDegConvert.DegreesToRadians(bearing);
            double v = RadDegConvert.DegreesToRadians(Lat.Value);
            double vMax = Math.Acos(Math.Abs(Math.Sin(u) * Math.Cos(v)));
            return new GdDegree(RadDegConvert.RadiansToDegrees(vMax));
        }

        public GdDistance RhumbDistanceTo(GdLonLat point)
        {
            double u1 = RadDegConvert.DegreesToRadians(Lat.Value);
            double u2 = RadDegConvert.DegreesToRadians(point.Lat.Value);
            double delta_u = u2 - u1;
            double delta_v = RadDegConvert.DegreesToRadians(Math.Abs(point.Lon.Value - Lon.Value));
            // if dLon over 180° take shorter rhumb line across the anti-meridian:
            if (delta_v > Math.PI) delta_v -= 2 * Math.PI;
            // on Mercator projection, longitude distances shrink by latitude; q is the 'stretch factor'
            // q becomes ill-conditioned along E-W line (0/0); use empirical tolerance to avoid it
            double delta_r = Math.Log(Math.Tan(u2 / 2 + Math.PI / 4) / Math.Tan(u1 / 2 + Math.PI / 4));
            double q = Math.Abs(delta_r) > 10e-12 ? delta_u / delta_r : Math.Cos(u1);

            // distance is pythagoras on 'stretched' Mercator projection
            double delta_ = Math.Sqrt(delta_u * delta_u + q * q * delta_v * delta_v); // angular distance in radians
            double dist = delta_ * Constants.RADIUS;

            return new GdDistance(dist);
        }

        public GdBearing RhumbBearingTo(GdLonLat point)
        {
            double u1 = RadDegConvert.DegreesToRadians(Lat.Value);
            double u2 = RadDegConvert.DegreesToRadians(point.Lat.Value);
            double delta_v = RadDegConvert.DegreesToRadians(point.Lon.Value - Lon.Value);
            // if dLon over 180° take shorter rhumb line across the anti-meridian:
            if (delta_v > Math.PI) delta_v -= 2 * Math.PI;
            if (delta_v < -Math.PI) delta_v += 2 * Math.PI;

            double delta_r = Math.Log(Math.Tan(u2 / 2 + Math.PI / 4) / Math.Tan(u1 / 2 + Math.PI / 4));

            double q = Math.Atan2(delta_v, delta_r);

            return new GdBearing((RadDegConvert.RadiansToDegrees(q) + 360) % 360);
        }

        public GdLonLat RhumbDestinationPoint(double distance, double bearing)
        {
            double f = distance / Constants.RADIUS; // angular distance in radians
            double u1 = RadDegConvert.DegreesToRadians(Lat.Value);
            double v1 = RadDegConvert.DegreesToRadians(Lon.Value);
            double y = RadDegConvert.DegreesToRadians(bearing);
            double delta_u = f * Math.Cos(y);
            double u2 = u1 + delta_u;

            // check for some daft bugger going past the pole, normalise latitude if so
            if (Math.Abs(u2) > Math.PI / 2) u2 = u2 > 0 ? Math.PI - u2 : -Math.PI - u2;

            double delta_z = Math.Log(Math.Tan(u2 / 2 + Math.PI / 4) / Math.Tan(u1 / 2 + Math.PI / 4));
            double q = Math.Abs(delta_z) > 10e-12 ? delta_u / delta_z : Math.Cos(u1); // E-W course becomes ill-conditioned with 0/0
            double delta_v = f * Math.Sin(y) / q;
            double v2 = v1 + delta_v;
            double latValue = RadDegConvert.RadiansToDegrees(u2);
            double lonValue = ((RadDegConvert.RadiansToDegrees(v2) + 540) % 360) - 180;
            return new GdLonLat()
            {
                Lon = new GdDegree(lonValue),
                Lat = new GdDegree(latValue)
            }; // normalise to −180..+180°
        }

        public GdLonLat RhumbMidpointTo(GdLonLat point)
        {
            double a1 = RadDegConvert.DegreesToRadians(Lat.Value);
            double b1 = RadDegConvert.DegreesToRadians(Lon.Value);
            double a2 = RadDegConvert.DegreesToRadians(point.Lat.Value);
            double b2 = RadDegConvert.DegreesToRadians(point.Lon.Value);

            if (Math.Abs(b2 - b1) > Math.PI) b1 += 2 * Math.PI; // crossing anti-meridian

            double a3 = (a1 + a2) / 2;
            double f1 = Math.Tan(Math.PI / 4 + a1 / 2);
            double f2 = Math.Tan(Math.PI / 4 + a2 / 2);
            double f3 = Math.Tan(Math.PI / 4 + a3 / 2);
            double b3 = ((b2 - b1) * Math.Log(f3) + b1 * Math.Log(f2) - b2 * Math.Log(f1)) / Math.Log(f2 / f1);

            if (!IsFinite(b3)) b3 = (b1 + b2) / 2; // parallel of latitude

            double latValue = RadDegConvert.RadiansToDegrees(a3);
            double lonValue = ((RadDegConvert.RadiansToDegrees(b3) + 540) % 360) - 180;

            return new GdLonLat()
            {
                Lon = new GdDegree(lonValue),
                Lat = new GdDegree(latValue)
            }; // normalise to −180..+180°
        }

        public bool Equals2D(object obj)
        {
            if (!(obj is GdLonLat))
                return false;


            GdLonLat lonLat = (GdLonLat)obj;

            return Lat.Value == lonLat.Lat.Value && Lon.Value == lonLat.Lon.Value;
        }

        public override string ToString()
        {
            return $"{Lon}, {Lat}";
        }

        private bool IsFinite(double v)
        {
            return Math.Abs(v) <= double.MaxValue;
        }
    }
}
