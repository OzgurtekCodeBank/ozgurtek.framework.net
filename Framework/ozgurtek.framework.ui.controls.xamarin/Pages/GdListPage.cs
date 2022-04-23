using System;
using System.Timers;
using ozgurtek.framework.ui.controls.xamarin.Views;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace ozgurtek.framework.ui.controls.xamarin.Pages
{
    public class GdListPage : GdPage
    {
        public readonly GdListView ListView;
        private readonly StackLayout _layout;
        private readonly Timer _searchTimer = new Timer();

        public GdListPage()
        {
            ListView = new GdListView();
            _layout = new StackLayout
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Padding = 0,
                Spacing = 5
            };
            _layout.Children.Add(ListView);
            DialogContent.Content = _layout;
        }

       
        public void AddSearchBox()
        {
            GdTextEntry searchInpView = new GdTextEntry();
            searchInpView.Placeholder = "Arama";
            searchInpView.TextChanged += OnTextChanged;
            _searchTimer.Interval = 1500;
            _searchTimer.AutoReset = false;
            _searchTimer.Enabled = false;
            _searchTimer.Elapsed += (se, ee) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    foreach (var item in ListView.Items)
                    {
                        item.IsVisible = item.Label.Text.ToLower().Contains(searchInpView.Text.ToLower());
                    }
                });
            };
            _layout.Children.Insert(0, searchInpView);
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_searchTimer.Enabled)
                _searchTimer.Enabled = true;

            _searchTimer.Stop();
            _searchTimer.Start();
        }
    }
}