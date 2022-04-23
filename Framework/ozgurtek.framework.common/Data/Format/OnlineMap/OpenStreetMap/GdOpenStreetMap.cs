using System;

namespace ozgurtek.framework.common.Data.Format.OnlineMap.OpenStreetMap
{
    public class GdOpenStreetMap : GdOnlineMap
    {
        readonly string UrlFormat = "https://tile.openstreetmap.org/{0}/{1}/{2}.png";

        public override string Name
        {
            get { return "OpenStreetMap"; }
        }

        public override Uri GetUri(long x, long y, int zoomLevel)
        {
            string format = string.Format(UrlFormat, zoomLevel, x, y);
            return new Uri(format);
        }
    }
}
