using System;

namespace ozgurtek.framework.common.Data.Format.OnlineMap.Google
{
    public class GdGoogleMap : GdAbstractGoogle
    {
        private readonly string _urlFormatServer = "mt";
        private readonly string _urlFormat = "https://{0}{1}.{10}/{2}/lyrs={3}&hl={4}&x={5}{6}&y={7}&z={8}&s={9}";
        private readonly string _urlFormatRequest = "vt";
        private readonly string _version = "m@221000000";

        public override string Name
        {
            get { return "GoogleMap"; }
        }

        public override Uri GetUri(long x, long y, int zoomLevel)
        {
            SecureWords sw = GetSecureWords(x, y);
            string sec1 = sw.GetSec1(); // after &x=...
            string sec2 = sw.GetSec2(); // after &zoom=...
            string format = string.Format(_urlFormat,
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
