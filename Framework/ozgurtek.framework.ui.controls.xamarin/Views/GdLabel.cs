using Xamarin.Forms;

namespace ozgurtek.framework.ui.controls.xamarin.Views
{
    public class GdLabel : Label
    {
        public TapGestureRecognizer Gesture;
        private object _tag;

        public GdLabel()
        {
            Gesture = new TapGestureRecognizer();
            LineBreakMode = LineBreakMode.TailTruncation;
            GestureRecognizers.Add(Gesture);

            this.SetAppThemeColor(Label.TextColorProperty, Color.Black, Color.Black);
        }

        public object Tag
        {
            get => _tag;
            set => _tag = value;
        }
    }
}
