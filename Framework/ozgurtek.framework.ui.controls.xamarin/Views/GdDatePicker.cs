using Xamarin.Forms;

namespace ozgurtek.framework.ui.controls.xamarin.Views
{
    public class GdDatePicker : DatePicker
    {
        private object _tag;

        public object Tag
        {
            get => _tag;
            set => _tag = value;
        }
    }
}
