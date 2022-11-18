using System;
using ozgurtek.framework.common.Data.Format.ArcGis;
using ozgurtek.framework.common.Data.Format.Wmts;

namespace ozgurtek.framework.common.Data.Format.Wmst
{
    public class GdWmtsMap : GdAbstractTileMap
    {
        private readonly string _address;
        private LayerType _capabilities;
        private string _format;
        private string _style;
        private string _appendGetMapRequest;

        public GdWmtsMap(string address)
        {
            _address = address;
        }

        public override string ConnectionString
        {
            get { return _address; }
        }

        public string Style
        {
            get => _style;
            set => _style = value;
        }

        public override Uri GetUri(long x, long y, int zoomLevel)
        {
            string getTileFormat = "{0}" +
                                   "?service=WMTS&version=1.0.0&request=GetTile" +
                                   "&layer={1}" +
                                   "&style={2}" +
                                   "&format={3}" +
                                   "&tileMatrixSet={4}" +
                                   "&tileMatrix={4}:{5}" +
                                   "&TileRow={6}" +
                                   "&TileCol={7}";
                                  

            string result = string.Format(getTileFormat,
                _address,
                Name,
                _style,
                _format,
                TileMatrixSet.Name,
                zoomLevel,
                y,
                x);

            if (Arcgistoken != null)
                result += "&token=" + Arcgistoken.Token;

            return new Uri(result);
        }

        public GdArcGisToken Arcgistoken { get; set; }

        public override string Format
        {
            get => _format;
            set => _format = value;
        }

        public LayerType Capabilities
        {
            get => _capabilities;
            set => _capabilities = value;
        }

        public string AppendGetMapRequest
        {
            get => _appendGetMapRequest;
            set => _appendGetMapRequest = value;
        }

        public override string ToString()
        {
            string result = string.Empty;
            if (!string.IsNullOrWhiteSpace(Name))
                result += Name;

            if (!string.IsNullOrWhiteSpace(Title))
                result += "--" + Title;

            if (!string.IsNullOrWhiteSpace(TileMatrixSet.Name))
                result += "--" + TileMatrixSet.Name;

            result += "--" + Srid;

            if (!string.IsNullOrWhiteSpace(Format))
                result += "--" + Format;

            if (!string.IsNullOrWhiteSpace(Style))
                result += "--" + Style;

            return result;
        }
    }
}