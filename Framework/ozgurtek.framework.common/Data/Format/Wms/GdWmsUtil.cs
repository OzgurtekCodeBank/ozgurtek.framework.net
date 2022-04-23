using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NetTopologySuite.Geometries;

namespace ozgurtek.framework.common.Data.Format.Wms
{
    public static class GdWmsUtil
    {
        /// <summary>
        /// Always return one style
        /// </summary>
        public static string[] ParseStyle(Style[] styles)
        {
            List<string> results = new List<string>();
            if (styles != null)
            {
                foreach (Style style in styles)
                    results.Add(style.Name);
            }

            if (results.Count == 0)
                results.Add(string.Empty);

            return results.ToArray();
        }

        /// <summary>
        /// Always return one srid
        /// </summary>
        public static int[] ParseSrids(string[] val)
        {
            List<int> results = new List<int>();
            
            if (val != null)
            {
                foreach (string s in val)
                {
                    int? srid = ParseSrid(s);
                    if (!srid.HasValue)
                        continue;

                    results.Add(srid.Value);
                }
            }

            if (results.Count == 0)
                results.Add(0);

            return results.ToArray();
        }

        public static int? ParseSrid(string val)
        {
            if (string.IsNullOrWhiteSpace(val))
                return null;

            if (val.IndexOf("EPSG", StringComparison.OrdinalIgnoreCase) < 0)
                return null;

            List<char> chars = new List<char>();
            for (int i = val.Length - 1; i > 0; i--)
            {
                if (char.IsDigit(val[i]))
                    chars.Add(val[i]);
                else
                    break;
            }

            //todo: gelen epsg kodu projection yaratabiliyor mu?
            chars.Reverse();
            string charsStr = new string(chars.ToArray());
            if (!int.TryParse(charsStr, out var code))
                return null;

            return code;
        }

        public static Envelope ParseEnvelope(BoundingBox[] boxTypes, int srid)
        {
            if (boxTypes == null)
                return null;

            foreach (BoundingBox boundingBoxType in boxTypes)
            {
                int? srid1 = ParseSrid(boundingBoxType.CRS);
                if (!srid1.HasValue)
                    continue;

                if (srid1 != srid)
                    continue;

                return ParseEnvelope(boundingBoxType);
            }

            return null;
        }

        public static Envelope ParseEnvelope(BoundingBox boxTypes)
        {
            return new Envelope(boxTypes.miny, boxTypes.maxy, boxTypes.minx, boxTypes.maxx);
        }

        public static string ParseQueryFormat(WMS_Capabilities capabilities)
        {
            if (capabilities == null)
                return null;

            if (capabilities.Capability == null)
                return null;

            if (capabilities.Capability.Request == null)
                return null;

            OperationType getFeatureInfo = capabilities.Capability.Request.GetFeatureInfo;
            if (getFeatureInfo == null)
                return null;

            string[] formats = getFeatureInfo.Format;
            if (formats == null)
                return null;

            foreach (string format in formats)
            {
                if (string.Equals(format, "application/json", StringComparison.OrdinalIgnoreCase))
                    return "application/json";

                if (string.Equals(format, "application/geojson", StringComparison.OrdinalIgnoreCase))
                    return "application/geojson";
            }
            return null;
        }
    }
}