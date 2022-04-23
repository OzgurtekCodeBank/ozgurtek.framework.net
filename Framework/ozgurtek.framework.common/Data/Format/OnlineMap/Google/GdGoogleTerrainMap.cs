using System;

namespace ozgurtek.framework.common.Data.Format.OnlineMap.Google
{
    public class GdGoogleTerrainMap : GdAbstractGoogle
    {
        readonly string _urlFormatServer = "mt";
        readonly string _urlFormatRequest = "vt";
        readonly string _urlFormat = "https://{0}{1}.{10}/maps/{2}/lyrs={3}&hl={4}&x={5}{6}&y={7}&z={8}&s={9}";
        readonly string _version = "t@132,r@333000000";

        public override string Name
        {
            get { return "GoogleTerrainMap"; }
        }

        public override Uri GetUri(long x, long y, int zoomLevel)
        {
            SecureWords sw = GetSecureWords(x, y);
            string sec1 = sw.GetSec1(); // after &x=...
            string sec2 = sw.GetSec2(); // after &zoom=...
            string format = string.Format(
                _urlFormat,
                _urlFormatServer,
                GetServerNum(x, y, 4),
                _urlFormatRequest,
                _version,
                Language,
                x,
                sec1,
                y,
                zoomLevel,
                sec2,
                Server);
            return new Uri(format);
        }
    }
}
