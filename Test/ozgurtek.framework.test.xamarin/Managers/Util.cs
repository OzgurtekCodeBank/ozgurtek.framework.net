using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using NetTopologySuite.Geometries;
using ozgurtek.framework.common;
using ozgurtek.framework.common.Data;
using ozgurtek.framework.common.Data.Format;
using ozgurtek.framework.core.Data;
using ozgurtek.framework.core.Mapping;
using Xamarin.Essentials;
using Location = Xamarin.Essentials.Location;

namespace ozgurtek.framework.test.xamarin.Managers
{
    public class UtilityManager
    {
        public void CallNumber(string number)
        {
            if (!number.StartsWith("0"))
                number = "0" + number;
            PhoneDialer.Open(number);
        }

        public void CheckInternetConnection()
        {
            NetworkAccess networkAccess = Connectivity.NetworkAccess;
            if (networkAccess != NetworkAccess.Internet)
                throw new Exception("İnternet bağlantısı yok");
        }

        public async void SendEmail(string emailAdress)
        {
            await Launcher.OpenAsync(new Uri($"mailto:{emailAdress}"));
        }

        public void Speech(string text)
        {
            var settings = new SpeechOptions
            {
                Volume = .75f,
                Pitch = 1.0f,
            };

            TextToSpeech.SpeakAsync(text, settings)
                .ContinueWith((t) => { }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        public Polygon CreatePolygonFromPixel(Coordinate centerInPixels, double boxInPixel = 20)
        {
            double buffer = boxInPixel * DeviceDisplay.MainDisplayInfo.Density; //pixel buffer
            Coordinate ll = new Coordinate(centerInPixels.X - buffer / 2, centerInPixels.Y + buffer / 2); //sol alt
            Coordinate lr = new Coordinate(centerInPixels.X + buffer / 2, centerInPixels.Y + buffer / 2); //sağ alt
            Coordinate ur = new Coordinate(centerInPixels.X + buffer / 2, centerInPixels.Y - buffer / 2); //sağ ust
            Coordinate ul = new Coordinate(centerInPixels.X - buffer / 2, centerInPixels.Y - buffer / 2); //sol üst

            GeometryFactory geometryFactory = GdFactoryFinder.Instance.GeometryServices.CreateGeometryFactory();
            Polygon polygon = geometryFactory.CreatePolygon(new[] {ul, ur, lr, ll, ul});

            return polygon;
        }

        public Polygon CreateWorldPolygonFromPixels(Coordinate centerInPixels, IGdMap map, double boxInPixel = 20)
        {
            double buffer = boxInPixel * DeviceDisplay.MainDisplayInfo.Density; //pixel buffer
            Coordinate ll = new Coordinate(centerInPixels.X - buffer / 2, centerInPixels.Y + buffer / 2); //sol alt
            Coordinate lr = new Coordinate(centerInPixels.X + buffer / 2, centerInPixels.Y + buffer / 2); //sağ alt
            Coordinate ur = new Coordinate(centerInPixels.X + buffer / 2, centerInPixels.Y - buffer / 2); //sağ ust
            Coordinate ul = new Coordinate(centerInPixels.X - buffer / 2, centerInPixels.Y - buffer / 2); //sol üst

            Coordinate llw = map.Viewport.ViewtoWorld(ll);
            Coordinate lrw = map.Viewport.ViewtoWorld(lr);
            Coordinate urw = map.Viewport.ViewtoWorld(ur);
            Coordinate ulw = map.Viewport.ViewtoWorld(ul);

            GeometryFactory geometryFactory = GdFactoryFinder.Instance.GeometryServices.CreateGeometryFactory();
            Polygon polygon = geometryFactory.CreatePolygon(new[] {ulw, urw, lrw, llw, ulw});

            return polygon;
        }

        public async void DoNavigation(Point point)
        {
            try
            {
                CheckInternetConnection();
                Location location = new Location(point.Y, point.X);

                bool locPer = await GdApp.Instance.PermissionManager.CheckForLocationPermission();

                if (locPer) //konum açıksa
                {
                    MapLaunchOptions options = new MapLaunchOptions {NavigationMode = NavigationMode.Driving};
                    await Map.OpenAsync(location, options);
                }
                else //konum kapalıysa
                {
                    OpenGoogleMaps(point);
                }
            }
            catch (Exception exception)
            {
                GdApp.Instance.LogManager.LogException(exception);
            }
        }

        public void OpenGoogleMaps(Point location)
        {
            try
            {
                CheckInternetConnection();
                string xStr = location.X.ToString(CultureInfo.InvariantCulture);
                string yStr = location.Y.ToString(CultureInfo.InvariantCulture);
                string googleQuery = HttpUtility.UrlEncode($"{yStr},{xStr}");
                googleQuery = "https://www.google.com/maps/search/?api=1&query=" + googleQuery;
                GdApp.Instance.PlatformManager.OpenBrowser(googleQuery);
            }
            catch (Exception e)
            {
                GdApp.Instance.LogManager.LogException(e);
            }
        }

        public void OpenGoogleMaps(string adress)
        {
            try
            {
                CheckInternetConnection();
                string googleQuery = HttpUtility.UrlEncode(adress);
                googleQuery = "https://www.google.com/maps/search/?api=1&query=" + googleQuery;
                GdApp.Instance.PlatformManager.OpenBrowser(googleQuery);
            }
            catch (Exception e)
            {
                GdApp.Instance.LogManager.LogException(e);
            }
        }

        public async void ShareMapLocation(Point location, string title)
        {
            try
            {
                CheckInternetConnection();
                string xStr = location.X.ToString(CultureInfo.InvariantCulture);
                string yStr = location.Y.ToString(CultureInfo.InvariantCulture);
                string googleQuery = HttpUtility.UrlEncode($"{yStr},{xStr}");
                googleQuery = "https://www.google.com/maps/search/?api=1&query=" + googleQuery;
                await Share.RequestAsync(new ShareTextRequest()
                {
                    Text = googleQuery,
                    Title = title
                });
            }
            catch (Exception e)
            {
                GdApp.Instance.LogManager.LogException(e);
            }
        }

        public void OpenStreetView(Point location)
        {
            try
            {
                CheckInternetConnection();
                string xStr = location.X.ToString(CultureInfo.InvariantCulture);
                string yStr = location.Y.ToString(CultureInfo.InvariantCulture);
                string googleQuery = HttpUtility.UrlEncode($"{yStr},{xStr}");
                googleQuery = "https://www.google.com/maps/@?api=1&map_action=pano&viewpoint=" + googleQuery;
                GdApp.Instance.PlatformManager.OpenBrowser(googleQuery);
            }
            catch (Exception exception)
            {
                GdApp.Instance.LogManager.LogException(exception);
            }
        }

        public string Scramble(string password)
        {
            var md5 = new MD5CryptoServiceProvider();
            var encode = new UTF8Encoding();
            var scrambledbytes = md5.ComputeHash(encode.GetBytes(password));
            var encryptdata = new StringBuilder();
            foreach (var t in scrambledbytes)
            {
                encryptdata.Append(t.ToString());
            }

            return encryptdata.ToString();
        }
    }
}