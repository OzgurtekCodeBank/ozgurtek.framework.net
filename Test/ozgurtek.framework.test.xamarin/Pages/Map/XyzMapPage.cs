using NetTopologySuite.Geometries;
using ozgurtek.framework.common.Data.Format.OnlineMap.Google;
using ozgurtek.framework.common.Data.Format.Wmst;
using ozgurtek.framework.common.Geodesy;
using ozgurtek.framework.common.Mapping;
using ozgurtek.framework.core.Data;
using ozgurtek.framework.test.xamarin.Managers;
using ozgurtek.framework.ui.controls.xamarin.Pages;
using ozgurtek.framework.ui.map.skiasharp;
using Xamarin.Forms;

namespace ozgurtek.framework.test.xamarin.Pages.Map
{
    public class XyzMapPage : GdPage
    {
        private readonly GdSkMap _map;

        public XyzMapPage()
        {
            StackLayout mainLayout = new StackLayout();
            mainLayout.VerticalOptions = LayoutOptions.FillAndExpand;

            StackLayout buttonLayout = new StackLayout();
            buttonLayout.Orientation = StackOrientation.Vertical;
            mainLayout.Children.Add(buttonLayout);


            _map = new GdSkMap();
            _map.VerticalOptions = LayoutOptions.FillAndExpand;
            mainLayout.Children.Add(_map);

            DialogContent.Content = mainLayout;

            this.ShowTopbar("XyzMap");

            Initialize();
        }

        private void Initialize()
        {
            Envelope world4326 = new Envelope(35.474853515625, 31.9921875, 35.871246850027966, 37.33522435930639);
            Envelope envelope = GdProjection.Project(world4326, 4326, 3857);

            _map.Viewport.World = envelope;
            _map.Viewport.Srid = 3857;
            _map.BackColor = GdColor.Gray;

            _map.LayerCollection.Add(GdApp.Instance.Data.GetBaseMap("GoogleMap"));

            //GdWmtsMap map = new GdWmtsMap("https://kbs.konya.bel.tr/kbscache/service/wmts");
            //map.Name = "basemap";
            //map.Format = "image/gif";
            //GdGoogleMapsTileMatrixSet gdGoogleMapsTileMatrixSet = new GdGoogleMapsTileMatrixSet();
            //gdGoogleMapsTileMatrixSet.Name = "EPSG:900913";
            //map.TileMatrixSet = gdGoogleMapsTileMatrixSet;
            //map.Srid = 900913;
            //map.HttpDownloadInfo.UseDiskCache = true;
            //map.HttpDownloadInfo.UseMemoryCache = true;
            //map.HttpDownloadInfo.DiskCacheFolder = GdApp.Instance.Settings.CacheFolder;
            //GdTileLayer tileLayer = new GdTileLayer(map, map.Name);


            GdWmtsMap map = new GdWmtsMap("http://185.122.200.110:8080/geoserver/gwc/service/wmts");
            map.Name = "konya_ibs:itf_istasyonlar";
            map.Format = "image/png";
            GdGoogleMapsTileMatrixSet gdGoogleMapsTileMatrixSet = new GdGoogleMapsTileMatrixSet();
            gdGoogleMapsTileMatrixSet.Name = "EPSG:900913";
            map.TileMatrixSet = gdGoogleMapsTileMatrixSet;
            map.Srid = 900913;
            map.HttpDownloadInfo.UseDiskCache = true;
            map.HttpDownloadInfo.UseMemoryCache = true;
            map.HttpDownloadInfo.DiskCacheFolder = GdApp.Instance.Settings.CacheFolder;
            GdTileLayer tileLayer = new GdTileLayer(map, map.Name);

            _map.LayerCollection.Add(tileLayer);
        }
    }
}
