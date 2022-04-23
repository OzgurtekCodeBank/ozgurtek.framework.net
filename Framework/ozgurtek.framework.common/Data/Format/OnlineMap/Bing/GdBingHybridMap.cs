using System;

namespace ozgurtek.framework.common.Data.Format.OnlineMap.Bing
{
    public class GdBingHybridMap : GdAbstractBing
    {
        static readonly string UrlFormat = "https://ecn.t{0}.tiles.virtualearth.net/tiles/h{1}.jpeg?g={2}&mkt={3}&n=z{4}";

        public override string Name
        {
            get { return "BingHybrid"; }
        }

        public override Uri GetUri(long x, long y, int zoomLevel)
        {
            string key = TileXyToQuadKey(x, y, zoomLevel);
            string format = string.Format(UrlFormat, GetServerNum(x, y, 4), key, Version, Language, ForceSessionIdOnTileAccess ? "&key=" + SessionId : string.Empty);
            return new Uri(format);
        }
    }
}
