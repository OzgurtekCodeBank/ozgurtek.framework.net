using ozgurtek.framework.ui.controls.xamarin.Views;
using Xamarin.Forms;

namespace ozgurtek.framework.ui.controls.xamarin.Pages
{
    public class GdSelectPage : GdListPage
    {
        public GdListViewItem CreateItem(string text, ImageSource imageSource = null, int id = -1)
        {
            GdListViewItem item = new GdListViewItem();
            item.Label.Text = text;
            item.ItemId = id;

            Image image = new Image();
            image.HeightRequest = 25;
            image.Source = imageSource;
            item.AddLeft(image);

            ListView.Items.Add(item);

            return item;
        }

        public GdListViewItem CreateItem(Color color)
        {
            GdListViewItem item = new GdListViewItem();
            item.Children.Clear();
            //item.Label.Text = "falanfilan";
            item.BackgroundColor = color;
            item.HeightRequest = 30;
            item.HorizontalOptions = LayoutOptions.FillAndExpand;
            ListView.Items.Add(item);
            return item;
        }
    }
}
