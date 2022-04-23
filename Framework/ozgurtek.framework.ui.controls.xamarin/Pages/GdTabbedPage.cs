using System;
using System.Collections.Generic;
using System.Linq;
using ozgurtek.framework.ui.controls.xamarin.Views;
using Xamarin.Forms;

namespace ozgurtek.framework.ui.controls.xamarin.Pages
{
    public abstract class GdTabbedPage : GdPage
    {
        private readonly Color _btnColor = Color.Black;
        private readonly StackLayout _content;
        private readonly StackLayout _tabBtnStackLayout;
        private readonly List<GdButton> _buttons = new List<GdButton>();
        private double _btnWidth = 120;

        protected GdTabbedPage()
        {
            int bottomHeight = 40;

            Grid mainGrid = new Grid
            {
                RowDefinitions =
                {
                    new RowDefinition {Height = new GridLength(1, GridUnitType.Star)},
                    new RowDefinition {Height = new GridLength(bottomHeight, GridUnitType.Absolute)},
                },
                ColumnDefinitions =
                {
                    new ColumnDefinition {Width = new GridLength(1, GridUnitType.Star)},
                },
                BackgroundColor = Color.White
            };

            mainGrid.VerticalOptions = LayoutOptions.FillAndExpand;

            _content = new StackLayout()
            {
                Padding = 0
            };

            mainGrid.Children.Add(_content, 0, 0);

            _tabBtnStackLayout = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal
            };

            ScrollView tabBtnScrollView = new ScrollView()
            {
                Orientation = ScrollOrientation.Horizontal,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Never
            };

            tabBtnScrollView.Content = _tabBtnStackLayout;

            mainGrid.Children.Add(tabBtnScrollView, 0, 1);
            DialogContent.Content = mainGrid;
        }

        public void SetCurrentTab(int tab)
        {
            _buttons[tab].Gesture.SendTapped(new GdButton());
        }

        public StackLayout AddTab(string title)
        {
            GdButton button = CreateButton(title);
            button.Gesture.Tapped += (sender, args) =>
            {
                foreach (GdButton btn in _buttons.Where(b => b != button))
                {
                    btn.Label.TextColor = _btnColor;
                    btn.Label.FontAttributes = FontAttributes.None;
                }

                button.Label.TextColor = _btnColor;
                button.Label.FontAttributes = FontAttributes.Bold;

                _content.Children.Clear();

                View selectedTab = GetTab(title);
                _content.Children.Add(selectedTab);
            };

            _tabBtnStackLayout.Children.Add(button);
            _buttons.Add(button);
            
            return button;
        }

        private GdButton CreateButton(string text)
        {
            GdButton button = new GdButton();
            button.WidthRequest = _btnWidth;
            button.Label.Text = text;
            button.Label.FontSize = 12;

            return button;
        }

        public double ButtonWidth
        {
            get { return _btnWidth; }
            set { _btnWidth = value; }
        }

        protected abstract View GetTab(string title);
    }
}