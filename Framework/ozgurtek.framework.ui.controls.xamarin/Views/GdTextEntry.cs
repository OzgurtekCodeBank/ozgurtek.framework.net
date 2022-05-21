using ozgurtek.framework.ui.controls.xamarin.Helper;
using Xamarin.Forms;

namespace ozgurtek.framework.ui.controls.xamarin.Views
{
    public class GdTextEntry : Entry
    {
        private object _tag;

        public GdTextEntry()
        {
            FontSize = 14;
        }

        public void AddDenySpaceBehavior()
        {
            Behaviors.Add(new GdSpaceValidationBehavior());
        }

        public void AddInvalidPathCharBehavior()
        {
            Behaviors.Add(new GdInvalidPathCharValidationBehavior());
        }

        public object Tag
        {
            get => _tag;
            set => _tag = value;
        }
    }
}
