using System;
using System.Globalization;
using NetTopologySuite.Geometries;
using ozgurtek.framework.common.Data.Format.OnlineMap.Google;
using ozgurtek.framework.core.Data;

namespace ozgurtek.framework.common.Data.Format.Xyz
{
    public class GdXyzMap : GdAbstractTileMap
    {
        IGdTileMatrixSet _tileMatrixSet = new GdGoogleMapsTileMatrixSet();
        private string _urlFormat;

        public GdXyzMap(string urlFormat)
        {
            _urlFormat = urlFormat;
            //Name = urlFormat;
            //Title = urlFormat;
            //ConnectionString = _urlFormat;
            //Format = 
        }

        public override IGdTileMatrixSet TileMatrixSet
        {
            get { return _tileMatrixSet; }
            set { _tileMatrixSet = value; }
        }

        public override int Srid { get; set; } = 3857;

        public override Envelope Envelope { get; set; } =
            new Envelope(-20026376.39, 20026376.39, -20048966.10, 20048966.10);

        public override Uri GetUri(long x, long y, int zoomLevel)
        {
            string replace = _urlFormat.Replace("{", "");
            replace = replace.Replace("}", "");
            replace = replace.Replace("x", x.ToString(CultureInfo.InvariantCulture))
                .Replace("y", y.ToString(CultureInfo.InvariantCulture))
                .Replace("z", zoomLevel.ToString(CultureInfo.InvariantCulture));

            return new Uri(replace);
        }
    }
}