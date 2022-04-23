using NetTopologySuite.Geometries;
using System;
using System.Globalization;
using System.Net.Http;
using ozgurtek.framework.common.Data.Format.ArcGis;
using ozgurtek.framework.core.Data;

namespace ozgurtek.framework.common.Data.Format.Wms
{
    public class GdWmsMap : IGdWmsMap
    {
        private IGdWmsMap[] _maps;
        private Layer _capabilities;
        private readonly string _connectionString;
        private bool _transparent = true;
        private string _styles = string.Empty;
        private string _format = "image/png";
        private string _infoFormat = "application/json";
        private GdArcGisToken _arcgistoken;
        private string _getMapGetMapParameters = string.Empty;
        private string _getFeatureInfoParameters = string.Empty;
        private readonly IGdHttpDownloadInfo _info = new GdHttpDownloadInfo();
        private HttpClient _client;

        public GdWmsMap(string connectionString)
        {
            _connectionString = connectionString;
        }

        public virtual string Title { get; set; }

        public virtual IGdHttpDownloadInfo HttpDownloadInfo
        {
            get { return _info; }
        }

        public virtual int Srid { get; set; }
        public virtual Envelope Envelope { get; set; }
        public virtual string Name { get; set; }

        public virtual string ConnectionString
        {
            get { return _connectionString; }
        }

        public Uri GetUri(Envelope world, int width, int height)
        {
            string envStr = string.Format("{0},{1},{2},{3}",
                world.MinY.ToString(CultureInfo.InvariantCulture),
                world.MinX.ToString(CultureInfo.InvariantCulture),
                world.MaxY.ToString(CultureInfo.InvariantCulture),
                world.MaxX.ToString(CultureInfo.InvariantCulture));

            string getMapFormat = "{0}" +
                                  "?request=GetMap&service=WMS&version=1.3.0" +
                                  "&layers={1}" +
                                  "&styles={2}" +
                                  "&crs=EPSG:{3}" +
                                  "&bbox={4}" +
                                  "&width={5}" +
                                  "&height={6}" +
                                  "&format={7}"+
                                  "&TRANSPARENT={8}";

            string result = string.Format(getMapFormat,
                _connectionString,
                Name,
                _styles,
                Srid,
                envStr,
                width,
                height,
                _format,
                _transparent);

            if (_arcgistoken != null)
                result += "&token=" + _arcgistoken.Token;

            if (!string.IsNullOrWhiteSpace(GetMapParameters))
                result += "&" + GetMapParameters;
            
            return new Uri(result);
        }

        public IGdTable GetFeatureInfo(Envelope envelope, int width, int height, int x, int y, int featureCount = 1)
        {
            if (_infoFormat == null)
                return null;
            
            if (Envelope != null && !envelope.Intersects(Envelope))
                return null;

            if (_client == null)
            {
                _client = new HttpClient();
                _client.Timeout = TimeSpan.FromMilliseconds(HttpDownloadInfo.HttpConnectTimeOut);
                _client.DefaultRequestHeaders.Add("User-Agent", HttpDownloadInfo.UserAgent);

                if (!string.IsNullOrWhiteSpace(HttpDownloadInfo.RefererUrl))
                    _client.DefaultRequestHeaders.Add("Referer", HttpDownloadInfo.RefererUrl);
            }

            string envStr = string.Format("{0},{1},{2},{3}",
                envelope.MinY.ToString(CultureInfo.InvariantCulture),
                envelope.MinX.ToString(CultureInfo.InvariantCulture),
                envelope.MaxY.ToString(CultureInfo.InvariantCulture),
                envelope.MaxX.ToString(CultureInfo.InvariantCulture));

            string getMapFormat = "{0}" +
                                  "?request=GetFeatureInfo&service=WMS&version=1.3.0" +
                                  "&layers={1}" +
                                  "&styles={2}" +
                                  "&crs=EPSG:{3}" +
                                  "&bbox={4}" +
                                  "&width={5}" +
                                  "&height={6}" +
                                  "&format={7}" +
                                  "&query_layers={1}" +
                                  "&info_format={8}" +
                                  "&feature_count={9}" +
                                  "&x={10}" +
                                  "&y={11}";

            string result = string.Format(getMapFormat,
                _connectionString,
                Name,
                _styles,
                Srid,
                envStr,
                width,
                height,
                _format,
                _infoFormat,
                featureCount,
                x,
                y);

            if (_arcgistoken != null)
                result += "&token=" + _arcgistoken.Token;

            if (!string.IsNullOrWhiteSpace(GetFeatureInfoParameters))
                result += "&" + GetFeatureInfoParameters;

            string json = string.Empty;
            try
            {
                json = _client.GetStringAsync(result).Result;
                GdMemoryTable table = GdMemoryTable.LoadFromJson(json, false);
                table.Name = Name;
                return table;
            }
            catch (Exception e)
            {
                throw new System.Exception(e.Message + " server return: -> " + json);
            }
        }

        public bool Transparent
        {
            get => _transparent;
            set => _transparent = value;
        }

        public string Styles
        {
            get => _styles;
            set => _styles = value;
        }

        public string Format
        {
            get => _format;
            set => _format = value;
        }

        public Layer Capabilities
        {
            get => _capabilities;
            set => _capabilities = value;
        }

        public IGdWmsMap[] Maps
        {
            get { return _maps; }
            set { _maps = value; }
        }

        public GdArcGisToken Arcgistoken
        {
            get => _arcgistoken;
            set => _arcgistoken = value;
        }

        public string InfoFormat
        {
            get => _infoFormat;
            set => _infoFormat = value;
        }

        public string GetMapParameters
        {
            get => _getMapGetMapParameters;
            set => _getMapGetMapParameters = value;
        }

        public string GetFeatureInfoParameters
        {
            get => _getFeatureInfoParameters;
            set => _getFeatureInfoParameters = value;
        }

        public override string ToString()
        {
            string result = string.Empty;
            if (!string.IsNullOrWhiteSpace(Name))
                result += Name;

            if (!string.IsNullOrWhiteSpace(Title))
                result += "--" + Title;

            result += "--" + Srid;

            if (!string.IsNullOrWhiteSpace(Styles))
                result += "--" + Styles;

            return result;
        }
    }
}