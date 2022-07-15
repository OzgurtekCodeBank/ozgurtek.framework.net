using System;
using ozgurtek.framework.test.xamarin.Managers;
using ozgurtek.framework.ui.controls.xamarin.Pages;
using ozgurtek.framework.ui.controls.xamarin.Views;
using Xamarin.Forms;

namespace ozgurtek.framework.test.xamarin.Pages
{
    public class GdSelectPageSample1 : GdListPage
    {
        public GdSelectPageSample1()
        {
            AddSearchBox();

            this.ShowTopbar("Sayı");

            for (int i = 0; i < 100; i++)
            {
                ImageSource imageSource = ImageSource.FromFile("bina.png");
                ListView.CreateItem("building" + i, imageSource);
            }

            ListView.ItemClicked += ItemClicked;
            SearchFinished += OnSearchFinished;
            WriteCaption();
        }

        private void OnSearchFinished(object sender, EventArgs e)
        {
            WriteCaption();  
        }

        private void WriteCaption()
        {
            int count = 0;
            foreach (GdListViewItem item in ListView.Items)
            {
                if (item.IsVisible)
                    count++;
            }

            DialogLabel.Text = "Count " + count;
        }

        private void ItemClicked(object sender, GdListViewItem e)
        {
            DisplayAlert("message", e.Label.Text, "ok", "cancel");
        }
    }
}