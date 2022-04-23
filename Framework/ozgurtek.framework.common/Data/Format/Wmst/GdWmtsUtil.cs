using System;
using System.Collections.Generic;
using System.Globalization;
using NetTopologySuite.Geometries;
using ozgurtek.framework.common.Data.Format.Wmts;

namespace ozgurtek.framework.common.Data.Format.Wmst
{
    public static class GdWmtsUtil
    {
        public static string GetString(CodeType codeType)
        {
            if (codeType == null)
                return null;

            return codeType.Value;
        }

        public static Coordinate GetCoordinate(string coordinate)
        {
            if (coordinate == null)
                return null;

            string[] topLeft = coordinate.Split(' ');
            if (!double.TryParse(topLeft[1], NumberStyles.Any, CultureInfo.InvariantCulture, out var x))
                return null;

            if (!double.TryParse(topLeft[0], NumberStyles.Any, CultureInfo.InvariantCulture, out var y))
                return null;

            return new Coordinate(x, y);
        }

        public static Envelope GetEnvelope(BoundingBoxType boxType)
        {
            if (boxType == null)
                return null;

            string lowerCorner = boxType.LowerCorner;
            if (string.IsNullOrWhiteSpace(lowerCorner))
                return null;

            string upperCorner = boxType.UpperCorner;
            if (string.IsNullOrWhiteSpace(upperCorner))
                return null;

            string[] ll = lowerCorner.Split(' ');
            double minX = double.Parse(ll[0], CultureInfo.InvariantCulture);
            double minY = double.Parse(ll[1], CultureInfo.InvariantCulture);

            string[] ur = upperCorner.Split(' ');
            double maxX = double.Parse(ur[0], CultureInfo.InvariantCulture);
            double maxY = double.Parse(ur[1], CultureInfo.InvariantCulture);

            return new Envelope(minX, maxX, minY, maxY);
        }

        public static Envelope GetEnvelope(BoundingBoxType[] boxTypes, int srid)
        {
            if (boxTypes == null)
                return null;

            foreach (BoundingBoxType boundingBoxType in boxTypes)
            {
                int? srid1 = GetSrid(boundingBoxType.crs);
                if (!srid1.HasValue)
                    continue;
                
                if (srid1 != srid)
                    continue;

                return GetEnvelope(boundingBoxType);
            }

            return null;
        }

        public static int? GetSrid(string val)
        {
            if (string.IsNullOrWhiteSpace(val))
                return null;

            if (val.IndexOf("EPSG", StringComparison.OrdinalIgnoreCase) < 0)
                return null;

            List<char> chars = new List<char>();
            for (int i = val.Length-1; i > 0; i--)
            {
                if (char.IsDigit(val[i]))
                    chars.Add(val[i]);
                else
                    break;
            }

            chars.Reverse();
            string charsStr = new string(chars.ToArray());
            if (!int.TryParse(charsStr, out var code))
                return null;

            return code;
        }

        public static string GetString(LanguageStringType[] str)
        {
            if (str == null || str.Length == 0)
                return null;

            List<string> tList = new List<string>();
            foreach (LanguageStringType title in str)
            {
                if (string.IsNullOrWhiteSpace(title.Value))
                    continue;

                tList.Add(title.Value);
            }

            return string.Join(" ", tList);
        }
    }
}
