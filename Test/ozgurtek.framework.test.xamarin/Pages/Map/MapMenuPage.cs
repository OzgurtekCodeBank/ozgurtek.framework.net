using System;
using ozgurtek.framework.test.xamarin.Managers;
using ozgurtek.framework.ui.controls.xamarin.Pages;
using ozgurtek.framework.ui.controls.xamarin.Views;
using Xamarin.Forms;

namespace ozgurtek.framework.test.xamarin.Pages.Map
{
    public class MapMenuPage : GdPage
    {
        private readonly GdViewBox _viewBox;

        public MapMenuPage()
        {
            StackLayout stackLayout = new StackLayout();

            _viewBox = new GdViewBox();
            stackLayout.Children.Add(_viewBox);
            DialogContent.Content = stackLayout;

            this.ShowTopbar("Map");

            Init();
        }

        private void Init()
        {
            //basic map
            GdLabel mapLabel = new GdLabel();
            mapLabel.Text = "Google, Bing, OpenStreet Map";
            _viewBox.AddItem("OnlineMap", mapLabel);
            mapLabel.Gesture.Tapped += MapSamplesGestureOnTapped;
        }

        private void MapSamplesGestureOnTapped(object sender, EventArgs e)
        {
            OnlineMapPage page = new OnlineMapPage();
            page.ShowPage(null);
        }
    }
}
