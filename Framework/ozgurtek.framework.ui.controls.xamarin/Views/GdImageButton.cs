using Xamarin.Forms;

namespace ozgurtek.framework.ui.controls.xamarin.Views
{
    public class GdImageButton : Image
    {
        private object _tag;
        public TapGestureRecognizer Gesture;

        public GdImageButton()
        {
            InitalizeComponent();
        }

        private void InitalizeComponent()
        {
            Gesture = new TapGestureRecognizer();
            GestureRecognizers.Add(Gesture);
        }

        public object Tag
        {
            get => _tag;
            set => _tag = value;
        }
    }
}
