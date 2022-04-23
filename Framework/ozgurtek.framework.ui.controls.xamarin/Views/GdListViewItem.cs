using Xamarin.Forms;

namespace ozgurtek.framework.ui.controls.xamarin.Views
{
    public class GdListViewItem : StackLayout
    {
        private Label _label;
        private object _tag;
        private readonly StackLayout _left;
        private readonly StackLayout _right;
        private int _itemId;

        public GdListViewItem()
        {
            HorizontalOptions = LayoutOptions.FillAndExpand;
            VerticalOptions = LayoutOptions.FillAndExpand;
            Padding = 10;
            Orientation = StackOrientation.Horizontal;

            _left = new StackLayout()
            {
                HorizontalOptions = LayoutOptions.Start,
                Orientation = StackOrientation.Horizontal,
                WidthRequest = 40
            };

            Children.Add(_left);

            StackLayout labelAndRight = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand
            };

            StackLayout labelLayout = new StackLayout
            {
                HorizontalOptions = LayoutOptions.StartAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand
            };

            _label = new Label
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                LineBreakMode = LineBreakMode.TailTruncation
            };
            labelLayout.Children.Add(_label);

            labelAndRight.Children.Add(labelLayout);

            _right = new StackLayout
            {
                HorizontalOptions = LayoutOptions.EndAndExpand,
                Orientation = StackOrientation.Horizontal
            };
            labelAndRight.Children.Add(_right);
            Children.Add(labelAndRight);
        }

        public void AddLeft(View view)
        {
            _left.Children.Add(view);
        }

        public void AddRight(View view)
        {
            _right.Children.Add(view);
        }

        public Label Label
        {
            get => _label;
            set => _label = value;
        }

        public object Tag
        {
            get => _tag;
            set => _tag = value;
        }

        public int ItemId
        {
            get => _itemId;
            set => _itemId = value;
        }

        public override string ToString()
        {
            return _label.Text;
        }
    }
}