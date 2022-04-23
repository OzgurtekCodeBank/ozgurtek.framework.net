using System;

namespace ozgurtek.framework.common.Geodesy
{
    public class GdBearing : GdDegree
    {
        public GdBearing(double value)
            : base(value)
        {
        }

        public string ToCompassPointString(int precision = 3)
        {
            var r = ((Value % 360) + 360) % 360;
            string[] cardinals = { "N", "NNE", "NE", "ENE", "E", "ESE", "SE", "SSE", "S", "SSW", "SW", "WSW", "W", "WNW", "NW", "NNW" };
            double n = 4 * Math.Pow(2, precision - 1);
            double d = Math.Round(r * n / 360) % n * 16 / n;
            int i = (int)d;
            return cardinals[i];
        }
    }
}
