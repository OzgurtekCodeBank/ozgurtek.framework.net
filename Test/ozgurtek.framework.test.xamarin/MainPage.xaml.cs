using System;
using ozgurtek.framework.test.xamarin.Managers;
using ozgurtek.framework.test.xamarin.Pages.Map;
using ozgurtek.framework.ui.controls.xamarin.Views;
using Xamarin.Forms;

namespace ozgurtek.framework.test.xamarin
{
    public partial class MainPage : ContentPage
    {
        private GdViewBox _viewBox;
        public MainPage()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            _viewBox = new GdViewBox();
            Menu.Children.Add(_viewBox);

            //map
            GdLabel mapLabel = new GdLabel();
            mapLabel.Text = "Click to see map samples...";
            _viewBox.AddItem("Map Samples", mapLabel);
            mapLabel.Gesture.Tapped += MapSamplesGestureOnTapped;
        }

        private void MapSamplesGestureOnTapped(object sender, EventArgs e)
        {
            MapMenuPage mapMenu = new MapMenuPage();
            mapMenu.ShowPage(null);
        }
    }
}
