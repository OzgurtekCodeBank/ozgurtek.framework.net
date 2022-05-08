using System;
using ozgurtek.framework.test.xamarin.Managers;
using ozgurtek.framework.test.xamarin.Pages;
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

            //select page
            GdLabel selectPage = new GdLabel();
            selectPage.Text = "Click to see select page samples...";
            _viewBox.AddItem("Select Page", selectPage);
            selectPage.Gesture.Tapped += SelectPageGestureOnTapped;
        }

        private void SelectPageGestureOnTapped(object sender, EventArgs e)
        {
            GdSelectPageSample1 sample1 = new GdSelectPageSample1();
            sample1.ShowPage(null);
        }

        private void MapSamplesGestureOnTapped(object sender, EventArgs e)
        {
            MapMenuPage mapMenu = new MapMenuPage();
            mapMenu.ShowPage(null);
        }
    }
}
