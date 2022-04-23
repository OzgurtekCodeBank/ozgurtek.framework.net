using System;

namespace ozgurtek.framework.common.Data.Format.OnlineMap.Bing
{
    public class GdBingMap : GdAbstractBing
    {
        static readonly string UrlFormat = "https://ecn.t{0}.tiles.virtualearth.net/tiles/r{1}?g={2}&mkt={3}&lbl=l1&stl=h&shading=hill&n=z{4}";

        public override string Name
        {
            get { return "BingMap"; }
        }

        public override Uri GetUri(long x, long y, int zoomLevel)
        {
            string key = TileXyToQuadKey(x, y, zoomLevel);
            string format = string.Format(UrlFormat, GetServerNum(x, y, 4), key, Version, Language, ForceSessionIdOnTileAccess ? "&key=" + SessionId : string.Empty);
            return new Uri(format);
        }
    }
}
