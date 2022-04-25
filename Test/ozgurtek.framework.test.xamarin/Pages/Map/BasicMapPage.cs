using NetTopologySuite.Geometries;
using ozgurtek.framework.core.Data;
using ozgurtek.framework.test.xamarin.Managers;
using ozgurtek.framework.ui.controls.xamarin.Pages;
using ozgurtek.framework.ui.map.skiasharp;

namespace ozgurtek.framework.test.xamarin.Pages.Map
{
    public class BasicMapPage : GdPage
    {
        private readonly GdSkMap _map;

        public BasicMapPage()
        {
            _map = new GdSkMap();
            DialogContent.Content = _map;

            this.ShowTopbar("Map");

            Initialize();
        }

        private void Initialize()
        {
            _map.Viewport.World = new Envelope(35.474853515625, 31.9921875, 35.871246850027966, 37.33522435930639);
            _map.Viewport.Srid = 4326;
            _map.BackColor = GdColor.Gray;

            _map.LayerCollection.AddRange(GdApp.Instance.Data.BaseMaps());
            _map.Render();
        }
    }
}
