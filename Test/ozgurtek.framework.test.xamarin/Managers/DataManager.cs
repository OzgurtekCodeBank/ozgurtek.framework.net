using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;
using ozgurtek.framework.common.Data.Format;
using ozgurtek.framework.common.Data.Format.OnlineMap;
using ozgurtek.framework.common.Data.Format.Wms;
using ozgurtek.framework.common.Mapping;
using ozgurtek.framework.core.Mapping;

namespace ozgurtek.framework.test.xamarin.Managers
{
    public class DataManager
    {
        private GdServerDataSource _serverDataSource;

        private const string OverrideToken =
            "B%3dC%3d%3d%3bC%40%3e%3c%3a%3a%3b%3f%3cA%3d%40%c2%86%3a%3e%3b8%3d%3f8%3f8%3a%3b%c2%86%7co%7d%7fDons%7c%7co%c2%80y";

        public GdServerDataSource CreateNewServerDataSource()
        {
            _serverDataSource = new GdServerDataSource(new Uri(ServiceUrl), OverrideToken);
            _serverDataSource.SetOverrideToken(OverrideToken);
            return _serverDataSource;
        }

        public GdServerDataSource ServerDataSource
        {
            get
            {
                _serverDataSource.ClearParameters();
                return _serverDataSource;
            }
        }

        public List<IGdLayer> KatmanlarLayers()
        {
            List<IGdLayer> result = new List<IGdLayer>();

            GdWmsMap map = new GdWmsMap(WmsServiceUrl);
            map.Name = "itf_hidrant";
            map.Srid = 4326;
            map.Envelope = new Envelope(35.474853515625, 31.9921875, 35.871246850027966, 37.33522435930639);
            IGdLayer layer = new GdWmsLayer(map, "hidrant_poi_ariza");
            result.Add(layer);

            map = new GdWmsMap(WmsServiceUrl);
            map.Name = "itf_ulasim_zor_sokak";
            map.Srid = 4326;
            map.Envelope = new Envelope(35.474853515625, 31.9921875, 35.871246850027966, 37.33522435930639);
            layer = new GdWmsLayer(map, "ulasim_zor_sokak_poi");
            result.Add(layer);

            map = new GdWmsMap(WmsServiceUrl);
            map.Name = "itf_istasyonlar";
            map.Srid = 4326;
            map.Envelope = new Envelope(35.474853515625, 31.9921875, 35.871246850027966, 37.33522435930639);
            layer = new GdWmsLayer(map, "istasyonlar_poi");
            result.Add(layer);

            map = new GdWmsMap(WmsServiceUrl);
            map.Name = "itf_sorumluluk_alanlari";
            map.Srid = 4326;
            map.Envelope = new Envelope(35.474853515625, 31.9921875, 35.871246850027966, 37.33522435930639);
            layer = new GdWmsLayer(map, "sorumluluk_alanlari_poi");
            result.Add(layer);

            map = new GdWmsMap(WmsServiceUrl);
            map.Name = "itf_olay_rapor";
            map.Srid = 4326;
            map.Envelope = new Envelope(35.474853515625, 31.9921875, 35.871246850027966, 37.33522435930639);
            layer = new GdWmsLayer(map, "olay_rapor_poi");
            result.Add(layer);

            return result;
        }

        public IEnumerable<IGdTileLayer> BaseMaps()
        {
            IEnumerable<GdOnlineMap> availableMap = GdOnlineMap.GetAvailableMap();
            foreach (GdOnlineMap onlineMap in availableMap)
            {
                onlineMap.HttpDownloadInfo.UseDiskCache = true;
                onlineMap.HttpDownloadInfo.UseMemoryCache = true;
                onlineMap.HttpDownloadInfo.DiskCacheFolder = GdApp.Instance.Settings.CacheFolder;
                yield return new GdTileLayer(onlineMap, onlineMap.Name);
            }
        }

        public IGdTileLayer GetBaseMap(string name)
        {
            GdOnlineMap map = GdOnlineMap.Open(name);
            map.HttpDownloadInfo.UseDiskCache = true;
            map.HttpDownloadInfo.UseMemoryCache = true;
            map.HttpDownloadInfo.DiskCacheFolder = GdApp.Instance.Settings.CacheFolder;
            return new GdTileLayer(map, map.Name);
        }

        private static string WmsServiceUrl
        {
            get { return "http://185.122.200.110:8080/geoserver/mersin_ibs/wms"; }
        }

        private string ServiceUrl
        {
            get { return "http://212.175.105.122:94/GdSpatialService.svc"; }
        }
    }
}