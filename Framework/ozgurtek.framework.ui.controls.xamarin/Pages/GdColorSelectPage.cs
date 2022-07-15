using System;
using ozgurtek.framework.ui.controls.xamarin.Models;
using Xamarin.Forms;

namespace ozgurtek.framework.ui.controls.xamarin.Pages
{
    internal class GdColorSelectPage : GdListPage
    {
        public double Alpha { get; set; }
        public GdColorSelectPage()
        {
            WidthSize = new GdPageSize(200);
            HeightSize = new GdPageSize(300);

            Caption = "Basic Colors";
            ListView.SelectionMode = SelectionMode.Single;
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();

            ListView.CreateItem(Color.FromRgba(0, 0, 0, Alpha));
            ListView.CreateItem(Color.FromRgba(0, 0, 128, Alpha));
            ListView.CreateItem(Color.FromRgba(0, 128, 0, Alpha));
            ListView.CreateItem(Color.FromRgba(128, 0, 0, Alpha));
            ListView.CreateItem(Color.FromRgba(255, 255, 255, Alpha));
            ListView.CreateItem(Color.FromRgba(255, 255, 0, Alpha));
            ListView.CreateItem(Color.FromRgba(0, 255, 255, Alpha));
            ListView.CreateItem(Color.FromRgba(128, 0, 128, Alpha));
        }
    }
}
