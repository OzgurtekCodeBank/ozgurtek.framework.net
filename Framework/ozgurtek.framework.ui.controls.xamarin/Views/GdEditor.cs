using Xamarin.Forms;

namespace ozgurtek.framework.ui.controls.xamarin.Views
{
    public class GdEditor : Editor
    {
        private object _tag;

        public GdEditor()
        {
            FontSize = 14;
        }

        public object Tag
        {
            get => _tag;
            set => _tag = value;
        }
    }
}
