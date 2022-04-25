using System;
using NetTopologySuite.Geometries;
using ozgurtek.framework.core.Data;
using ozgurtek.framework.core.Mapping;
using ozgurtek.framework.test.xamarin.Managers;
using ozgurtek.framework.ui.controls.xamarin.Pages;
using ozgurtek.framework.ui.map.skiasharp;
using Xamarin.Forms;

namespace ozgurtek.framework.test.xamarin.Pages.Map
{
    public class OnlineMapPage : GdPage
    {
        private readonly GdSkMap _map;
        private readonly Button _button;
        private int _current;

        public OnlineMapPage()
        {
            StackLayout mainLayout = new StackLayout();
            mainLayout.VerticalOptions = LayoutOptions.FillAndExpand;

            StackLayout buttonLayout = new StackLayout();
            buttonLayout.Orientation = StackOrientation.Vertical;
            mainLayout.Children.Add(buttonLayout);

            _button = new Button();
            _button.Clicked += ButtonOnClicked;
            buttonLayout.Children.Add(_button);

            _map = new GdSkMap();
            _map.VerticalOptions = LayoutOptions.FillAndExpand;
            mainLayout.Children.Add(_map);

            DialogContent.Content = mainLayout;

            this.ShowTopbar("OnlineMap");

            Initialize();
        }

        private void ButtonOnClicked(object sender, EventArgs e)
        {
            AdjustVisible();
        }

        private void Initialize()
        {
            _map.Viewport.World = new Envelope(35.474853515625, 31.9921875, 35.871246850027966, 37.33522435930639);
            _map.Viewport.Srid = 4326;
            _map.BackColor = GdColor.Gray;

            _map.LayerCollection.AddRange(GdApp.Instance.Data.BaseMaps());
            AdjustVisible();
        }

        private void AdjustVisible()
        {
            foreach (IGdLayer layer in _map.LayerCollection)
                layer.Renderer.Style.Visible = false;

            IGdLayer mapLayer = _map.LayerCollection[_current++];
            mapLayer.Renderer.Style.Visible = true;
            if (_current >= _map.LayerCollection.Count)
                _current = 0;

            _map.Render();
            _button.Text = mapLayer.Name;
        }
    }
}