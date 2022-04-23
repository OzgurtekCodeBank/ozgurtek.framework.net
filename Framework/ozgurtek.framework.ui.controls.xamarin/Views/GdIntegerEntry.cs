using System.Globalization;
using ozgurtek.framework.common.Data;
using ozgurtek.framework.ui.controls.xamarin.Helper;
using Xamarin.Forms;

namespace ozgurtek.framework.ui.controls.xamarin.Views
{
    public class GdIntegerEntry : Entry
    {
        public GdIntegerEntry()
        {
            FontSize = 14;
            Behaviors.Add(new GdIntegerValidationBehavior());
        }

        public void AddMinMaxValueBehavior(int min, int max)
        {
            Behaviors.Add(new GdMinMaxNumberValidationBehavior(min, max));
        }

        public int? Value
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Text))
                    return null;

                return DbConvert.ToInt32(Text);
            }
            set
            {
                if (value == null)
                    Text = null;

                if (value != null) 
                    Text = value.Value.ToString(CultureInfo.InvariantCulture);
            }
        }
    }
}
