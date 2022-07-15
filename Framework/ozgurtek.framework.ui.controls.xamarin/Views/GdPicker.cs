using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ozgurtek.framework.ui.controls.xamarin.Models;
using ozgurtek.framework.ui.controls.xamarin.Pages;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;

namespace ozgurtek.framework.ui.controls.xamarin.Views
{
    public class GdPicker : Label
    {
        private object _tag;
        private TapGestureRecognizer _gesture;
        private GdListPage _page;
        private GdListViewItem _selectedItem;
        public EventHandler<GdListViewItem> SelectedItemChanged;

        public GdPicker()
        {
            InitializeComponent();
            PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IsEnabled))
            {
                BackgroundColor = IsEnabled ? Color.Transparent : Color.Gray;
            }
        }

        private void InitializeComponent()
        {
            _page = new GdListPage();
            _page.Caption = "Select";
            _page.WidthSize = new GdPageSize(200);
            _page.HeightSize = new GdPageSize(280);
            _gesture = new TapGestureRecognizer();
            LineBreakMode = LineBreakMode.TailTruncation;
            GestureRecognizers.Add(_gesture);
            _gesture.Tapped += GestureOnTapped;
            _page.ListView.SelectionMode = SelectionMode.Single;
            _page.ListView.ItemClicked += (sender, item) =>
            {
                Text = item.Label.Text;
                _selectedItem = item;
                PopupNavigation.Instance.PopAsync();
                OnSelectedItemChanged(item);
            };
        }

        public GdListPage SelectPage
        {
            get { return _page; }
        }

        public GdListViewItem SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                int indexOf = _page.ListView.Items.IndexOf(value);
                if (indexOf == -1)
                    throw new Exception("Value not find in list");

                _selectedItem = _page.ListView.Items[indexOf];
                Text = _selectedItem.Label.Text;
            }
        }

        public int SelectedIndex
        {
            get
            {
                return _page.ListView.Items.IndexOf(_selectedItem);
            }
            set
            {
                _selectedItem = _page.ListView.Items[value];
                Text = _selectedItem.Label.Text;
            }
        }

        public int SelectedId
        {
            get
            {
                return _selectedItem.ItemId;
            }
            set
            {
                _selectedItem = _page.ListView.Items.Single(i => i.ItemId == value);
                Text = _selectedItem.Label.Text;
            }
        }

        public List<GdListViewItem> Items
        {
            get { return _page.ListView.Items.ToList(); }
        }

        public GdListViewItem CreateItem(string text, ImageSource imageSource = null, int id = -1)
        {
            GdListViewItem item = _page.ListView.CreateItem(text, imageSource, id);
            return item;
        }

        public GdListViewItem CreateItem(string value, int key)
        {
            GdListViewItem item = _page.ListView.CreateItem(value, null, key);
            return item;
        }

        private async void GestureOnTapped(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PushAsync(_page);
        }

        protected void OnSelectedItemChanged(GdListViewItem item)
        {
            if (SelectedItemChanged != null)
                SelectedItemChanged(this, item);
        }

        public object Tag
        {
            get => _tag;
            set => _tag = value;
        }
    }
}
