using Xamarin.Forms;

namespace ozgurtek.framework.ui.controls.xamarin.Views
{
    public class GdViewBox : StackLayout
    {
        private readonly Color _defaultColor = Color.Black;
        private Grid _contentGrid;
        private double _titleWidth = 130;
        private string _header;
        private Frame _headerFrame;
        private Label _headerLabel;
        private ColumnDefinition _firstColumnDf;

        public GdViewBox()
        {
            Spacing = 0;
            Fill();
        }

        private void Fill()
        {
            _headerFrame = new Frame()
            {
                Padding = 5,
                CornerRadius = 0,
                Margin = new Thickness(20, 0, 0, 0),
                HorizontalOptions = LayoutOptions.Start,
                BorderColor = _defaultColor,
                IsVisible = false,
                HasShadow = false
            };

            _headerLabel = new Label()
            {
                TextColor = _defaultColor,
                Text = Header
            };

            _headerFrame.Content = _headerLabel;
            Children.Add(_headerFrame);

            Frame contentFrame = new Frame()
            {
                BorderColor = _defaultColor,
                Padding = 10,
                HasShadow = false,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand
            };

            _contentGrid = new Grid()
            {
                ColumnSpacing = 0,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand
            };

            _firstColumnDf = new ColumnDefinition { Width = new GridLength(TitleWidth, GridUnitType.Absolute) };
            _contentGrid.ColumnDefinitions.Add(_firstColumnDf);
            _contentGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            contentFrame.Content = _contentGrid;

            Children.Add(contentFrame);
        }

        public double TitleWidth
        {
            get { return _titleWidth; }
            set
            {
                _titleWidth = value;
                _headerFrame.WidthRequest = _titleWidth;
                _firstColumnDf.Width = new GridLength(_titleWidth, GridUnitType.Absolute);
            }
        }

        public string Header
        {
            get { return _header; }
            set
            {
                _header = value;
                _headerFrame.IsVisible = !string.IsNullOrEmpty(_header);
                _headerLabel.Text = _header;
            }
        }

        public void AddItem(string title, View view, double rowHeight = 1, GridUnitType unitType = GridUnitType.Auto)
        {
            _contentGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(rowHeight, unitType) });

            Frame titleFrame = new Frame
            {
                CornerRadius = 0,
                Padding = 5,
                Margin = 0,
                BorderColor = _defaultColor,
                HasShadow = false,
            };

            Label titleLabel = new Label
            {
                VerticalTextAlignment = TextAlignment.Center,
                VerticalOptions = LayoutOptions.Center,
                Text = title,
                HorizontalTextAlignment = TextAlignment.Start,
                HorizontalOptions = LayoutOptions.Start,
                FontAttributes = FontAttributes.Bold,
            };

            titleFrame.Content = titleLabel;

            Frame contentFrame = new Frame
            {
                CornerRadius = 0,
                Padding = 5,
                Margin = 0,
                BorderColor = _defaultColor,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Content = view,
                HasShadow = false
            };

            int currentRowIndex = _contentGrid.RowDefinitions.Count - 1;
            _contentGrid.Children.Add(titleFrame, 0, currentRowIndex);
            _contentGrid.Children.Add(contentFrame, 1, currentRowIndex);
        }

        public void ClearItems()
        {
            _contentGrid.Children.Clear();
            _contentGrid.RowDefinitions.Clear();
        }

    }
}