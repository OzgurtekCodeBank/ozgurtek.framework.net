using Xamarin.Forms;

namespace ozgurtek.framework.ui.controls.xamarin.Views
{
    public class GdImageButton : Image
    {
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
    }
}
