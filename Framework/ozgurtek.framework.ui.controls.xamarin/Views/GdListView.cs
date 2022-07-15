using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Xamarin.Forms;

namespace ozgurtek.framework.ui.controls.xamarin.Views
{
    public class GdListView : ScrollView
    {
        private object _tag;
        
        public EventHandler NeedMoreElement;
        public EventHandler<GdListViewItem> ItemClicked;
        
        public readonly ObservableCollection<GdListViewItem> Items;

        private readonly StackLayout _stackLayout;
        private Color _selectedColor = Color.LightGray;
        private SelectionMode _selectionMode = SelectionMode.Single;
        private readonly List<GdListViewItem> _selectedItems = new List<GdListViewItem>();
        
        public GdListView()
        {
            _stackLayout = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill
            };

            Items = new ObservableCollection<GdListViewItem>();
            Items.CollectionChanged += ItemsOnCollectionChanged;
            Scrolled += OnScrolled;

            Content = _stackLayout;
        }

        private void OnScrolled(object sender, ScrolledEventArgs e)
        {
            double scrollingSpace = (int)(ContentSize.Height - Height - 1);
            if (scrollingSpace <= e.ScrollY && NeedMoreElement != null)
                OnNeedMoreElements();
        }

        private void ItemsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                GdListViewItem item = e.NewItems[0] as GdListViewItem;
                if (item != null && SelectionMode != SelectionMode.None)
                {
                    TapGestureRecognizer recognizer = new TapGestureRecognizer();
                    recognizer.Tapped += (o, args) =>
                    {
                        OnItemClicked(item);
                    };
                    item.GestureRecognizers.Add(recognizer);
                }
                _stackLayout.Children.Add(item);
                return;
            }

            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                _stackLayout.Children.Clear();
            }

            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                var item = e.OldItems[0] as GdListViewItem;
                _stackLayout.Children.Remove(item);
            }
        }

        public void AddSelection(GdListViewItem item)
        {
            if (SelectionMode == SelectionMode.None)
                return;

            if (SelectionMode == SelectionMode.Single)
                _selectedItems.Clear();

            _selectedItems.Add(item);

            SetSelectedBackColor();
        }

        public void AddSeperator()
        {
            _stackLayout.Children.Add(new BoxView
            {
                Color = Color.FromHex("#95A5A6"),
                HeightRequest = 1,
                HorizontalOptions = LayoutOptions.FillAndExpand
            });
        }

        public void Clear()
        {
            _selectedItems.Clear();
            _stackLayout?.Children.Clear();
            Items.Clear();
        }

        public void RemoveSelection(GdListViewItem item)
        {
            if (SelectionMode == SelectionMode.None)
                return;

            _selectedItems.Remove(item);

            SetSelectedBackColor();
        }

        public SelectionMode SelectionMode
        {
            get => _selectionMode;
            set => _selectionMode = value;
        }

        public IList<GdListViewItem> SelectedItems
        {
            get { return _selectedItems; }
        }

        public Color SelectedColor
        {
            get => _selectedColor;
            set => _selectedColor = value;
        }

        private void SetSelectedBackColor()
        {
            foreach (View view in _stackLayout.Children)
            {
                if (view is GdListViewItem child)
                {
                    int indexOf = _selectedItems.LastIndexOf(child);
                    child.BackgroundColor = indexOf < 0 ? Color.Transparent : SelectedColor;
                }
            }
        }

        protected void OnItemClicked(GdListViewItem item)
        {
            if (_selectedItems.LastIndexOf(item) < 0)
                AddSelection(item);
            else
                RemoveSelection(item);

            if (ItemClicked != null)
                ItemClicked(this, item);
        }

        public object Tag
        {
            get => _tag;
            set => _tag = value;
        }

        public GdListViewItem CreateItem(string text, ImageSource imageSource = null, int id = -1)
        {
            GdListViewItem item = new GdListViewItem();
            item.Label.Text = text;
            item.ItemId = id;

            Image image = new Image();
            image.HeightRequest = 25;
            image.Source = imageSource;
            item.AddLeft(image);

            Items.Add(item);

            return item;
        }

        public GdListViewItem CreateItem(Color color)
        {
            GdListViewItem item = new GdListViewItem();
            item.Children.Clear();
            item.BackgroundColor = color;
            item.HeightRequest = 30;
            item.HorizontalOptions = LayoutOptions.FillAndExpand;
            Items.Add(item);
            return item;
        }

        protected void OnNeedMoreElements()
        {
            if (NeedMoreElement == null)
                return;

            NeedMoreElement(this, EventArgs.Empty);
        }
    }
}