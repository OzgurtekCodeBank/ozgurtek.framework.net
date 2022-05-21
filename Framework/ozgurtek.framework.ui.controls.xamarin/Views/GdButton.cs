using Xamarin.Forms;

namespace ozgurtek.framework.ui.controls.xamarin.Views
{
    public class GdButton : StackLayout
    {
        public TapGestureRecognizer Gesture;
        private GdLabel _label;
        private object _tag;

        public GdButton()
        {
            BackgroundColor = Color.White;
            _label = new GdLabel();
            _label.TextColor = Color.Black;
            _label.HorizontalOptions = LayoutOptions.Center;
            _label.HorizontalTextAlignment = TextAlignment.Center;
            _label.VerticalOptions = LayoutOptions.Center;
            _label.VerticalTextAlignment = TextAlignment.Center;
            Children.Add(_label);
            Gesture = new TapGestureRecognizer();
            GestureRecognizers.Add(Gesture);
            _label.GestureRecognizers.Add(Gesture);
        }

        public GdLabel Label
        {
            get { return _label; }
        }

        public object Tag
        {
            get => _tag;
            set => _tag = value;
        }
    }
}