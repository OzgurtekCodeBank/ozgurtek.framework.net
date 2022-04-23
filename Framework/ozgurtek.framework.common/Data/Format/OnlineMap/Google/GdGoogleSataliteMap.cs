using System;

namespace ozgurtek.framework.common.Data.Format.OnlineMap.Google
{
    public class GdGoogleSataliteMap : GdAbstractGoogle
    {
        private readonly string _url = "https://mts1.google.com/vt/lyrs=s@210119155&hl=en&src=app&x={0}&y={1}&z={2}";

        public override string Name
        {
            get { return "GoogleSataliteMap"; }
        }

        public override Uri GetUri(long x, long y, int zoomLevel)
        {
            string format = string.Format(_url, x, y, zoomLevel);
            return new Uri(format, UriKind.RelativeOrAbsolute);
        }
    }
}
