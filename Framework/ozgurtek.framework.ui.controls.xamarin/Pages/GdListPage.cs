using System;
using ozgurtek.framework.ui.controls.xamarin.Views;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace ozgurtek.framework.ui.controls.xamarin.Pages
{
    public class GdListPage : GdPage
    {
        public readonly GdListView ListView;
        private readonly StackLayout _layout;
        public SearchBar SearchBar;
        public event EventHandler SearchFinished;

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
            SearchBar = new SearchBar();
            SearchBar.TextChanged += SearchBarOnTextChanged;
            SearchBar.SearchButtonPressed += SearchBarOnSearchButtonPressed;
            _layout.Children.Insert(0, SearchBar);
        }

        private void SearchBarOnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(e.NewTextValue))
                return;

            SearchBarOnSearchButtonPressed(null, null);
        }

        private void SearchBarOnSearchButtonPressed(object o, object o1)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                foreach (var item in ListView.Items)
                {
                    item.IsVisible = item.Label.Text.ToLower().Contains(SearchBar.Text.ToLower());
                }

                OnSearchFinished();
            });
        }

        protected void OnSearchFinished()
        {
            if (SearchFinished != null)
                SearchFinished(this, EventArgs.Empty);
        }
    }
}