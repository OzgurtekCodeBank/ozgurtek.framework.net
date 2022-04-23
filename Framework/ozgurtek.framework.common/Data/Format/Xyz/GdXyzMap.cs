namespace ozgurtek.framework.common.Data.Format.Xyz
{
    public class GdXyzMap
    {
        //private string _urlToFormat;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="urlFormat">
        /// Url scheme. Example:
        /// www.provider.com/{x}/{y}/{z}.png</param>
        public GdXyzMap(string urlFormat)
        {
            //UrlToFormat = urlFormat;
        }

        //public override string Name { get; }

        //public override int Srid
        //{
        //    get { return 4326; }
        //    set
        //    {

        //    }
        //}

        //public override Envelope Envelope
        //{
        //    get { return new Envelope(-180.0, 180, -85.06, 85.06); }
        //}

        //public override Uri GetUri(long x, long y, int zoomLevel)
        //{
        //    return new Uri(string.Format(_urlToFormat, x, y, zoomLevel));
        //}

        //public string UrlToFormat
        //{
        //    get { return _urlToFormat; }
        //    set
        //    {
        //        _urlToFormat =
        //            value.Replace("{x}", "{0}")
        //                .Replace("{y}", "{1}")
        //                .Replace("{z}", "{2}");
        //    }
        //}

        //public override int MinZoomLevel
        //{
        //    get { return 1; }
        //}

        //public override int MaxZoomLevel
        //{
        //    get { return 21; }
        //}
    }
}