using System;
using System.Collections.Generic;
using System.Timers;
using NetTopologySuite.Geometries;
using ozgurtek.framework.common.Data.Format;
using ozgurtek.framework.core.Data;
using ozgurtek.framework.core.Mapping;
using ozgurtek.framework.test.xamarin.Managers;
using ozgurtek.framework.ui.controls.xamarin.Pages;
using ozgurtek.framework.ui.map.skiasharp;
using Xamarin.Forms;
using Point = NetTopologySuite.Geometries.Point;

namespace ozgurtek.framework.test.xamarin.Pages.Map
{
    public class MarkerMapPage : GdPage
    {
        private readonly GdSkMap _map;
        private bool _aracAnimationInProgress;

        public MarkerMapPage()
        {
            _map = new GdSkMap();
            _map.VerticalOptions = LayoutOptions.FillAndExpand;

            DialogContent.Content = _map;

            this.ShowTopbar("OnlineMap");

            Initialize();
        }

        private void Initialize()
        {
            _map.Viewport.World = new Envelope(35.474853515625, 31.9921875, 35.871246850027966, 37.33522435930639);
            _map.Viewport.Srid = 4326;
            _map.BackColor = GdColor.Gray;
            _map.LayerCollection.Add(GdApp.Instance.Data.GetBaseMap("GoogleMap"));

            Timer refreshMapTime = new Timer();
            refreshMapTime.Interval = 3000;
            refreshMapTime.Elapsed += RefreshMapTimeOnElapsed;
            refreshMapTime.Start();
        }

        private void RefreshMapTimeOnElapsed(object sender, ElapsedEventArgs e)
        {
            Refresh();
        }

        private void Refresh()
        {
            try
            {
                if (_aracAnimationInProgress)
                    return;

                _aracAnimationInProgress = true;

                //collect oldmarker
                List<ImageMarker> oldMarker = new List<ImageMarker>();
                foreach (IGdMarker marker in _map.Markers)
                {
                    if (marker is ImageMarker imageMarker)
                        oldMarker.Add(imageMarker);
                }

                GdServerDataSource dataSource = GdApp.Instance.Data.CreateNewServerDataSource();
                IGdTable table = dataSource.GetTable("pg:v1_itf_arac_takip");

                foreach (IGdRow row in table.Rows)
                {

                    if (!row.IsNull("geometry"))
                    {
                        string source = "hareketsiz.png";
                        Point point = (Point)row.GetAsGeometry("geometry");
                        ImageMarker _marker = new ImageMarker(point, source);
                        _map.Markers.Add(_marker);
                    }
                }

                //remove old marker
                foreach (ImageMarker imageMarker in oldMarker)
                {
                    imageMarker.Dispose();
                    _map.Markers.Remove(imageMarker);
                }
            }
            catch (Exception exception)
            {
                GdApp.Instance.LogManager.LogException(exception, true);
            }
            finally
            {
                _aracAnimationInProgress = false;
            }
        }
    }
}
