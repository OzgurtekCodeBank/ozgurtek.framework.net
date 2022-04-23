using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using NetTopologySuite.Geometries;
using ozgurtek.framework.common.Data.Format.ArcGis;
using ozgurtek.framework.core.Data;

namespace ozgurtek.framework.common.Data.Format.Wms
{
    public class GdWmsDataSource
    {
        private readonly string _address;
        private GdArcGisToken _arcgistoken;
        private string _cacheFolder;
        private List<GdWmsMap> _cache = new List<GdWmsMap>();

        public GdWmsDataSource(string address)
        {
            _address = address.Trim();
        }

        public void Open()
        {
            FillCache();
        }

        public string Address
        {
            get { return _address; }
        }

        public string CacheFolderForCapabilities
        {
            get => _cacheFolder;
            set => _cacheFolder = value;
        }

        public List<GdWmsMap> GetMap()
        {
            if (_cache == null)
                throw new System.Exception("open the datasource first");

            return _cache;
        }

        public List<GdWmsMap> GetMap(string name)
        {
            List<GdWmsMap> result = new List<GdWmsMap>();
            IEnumerable<GdWmsMap> maps = GetMap();
            foreach (GdWmsMap map in maps)
            {
                if (map.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                    result.Add(map);
            }
            return result;
        }

        public GdArcGisToken Arcgistoken
        {
            get => _arcgistoken;
            set => _arcgistoken = value;
        }

        public WMS_Capabilities GetCapabilities()
        {
            string urlString = $"{Address}{"?service=wms&version=1.3.0&request=GetCapabilities"}";
            if (_arcgistoken != null)
                urlString += $"&token={_arcgistoken.Token}";

            XmlReader reader = XmlReader.Create(urlString);
            XmlSerializer serializer = new XmlSerializer(typeof(WMS_Capabilities));
            WMS_Capabilities capabilities = (WMS_Capabilities) serializer.Deserialize(reader);

            return capabilities;
        }

        private void FillCache()
        {
            if (_cache.Count != 0)
                return;

            _cache = new List<GdWmsMap>();
            WMS_Capabilities capabilities = GetCapabilities();
            string queryFormat = GdWmsUtil.ParseQueryFormat(capabilities);
            Layer capabilityLayer = capabilities.Capability.Layer;
            if (capabilityLayer == null)
                return;
            
            Layer[] layers = capabilityLayer.Layer1;
            CreateMaps(layers, _cache, queryFormat);
        }

        private void CreateMaps(Layer[] layers, List<GdWmsMap> maps, string queryFormat)
        {
            if (layers == null)
                return;

            foreach (Layer layer in layers)
            {
                int[] srids = GdWmsUtil.ParseSrids(layer.CRS);
                foreach (int srid in srids)
                {
                    Envelope envelope = GdWmsUtil.ParseEnvelope(layer.BoundingBox, srid);

                    string[] styles = GdWmsUtil.ParseStyle(layer.Style);
                    foreach (string style in styles)
                    {
                        GdWmsMap map = new GdWmsMap(_address);
                        map.Name = layer.Name;
                        map.Title = layer.Title;
                        map.Arcgistoken = _arcgistoken;
                        map.Srid = srid;
                        map.Styles = style;
                        map.Capabilities = layer;
                        map.Envelope = envelope;
                        map.InfoFormat = queryFormat;
                        List<GdWmsMap> list = new List<GdWmsMap>();
                        CreateMaps(layer.Layer1, list, queryFormat);
                        map.Maps = list.ToArray();
                        maps.Add(map);
                    }
                }
            }
        }
    }
}