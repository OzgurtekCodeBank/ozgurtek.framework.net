using System.Globalization;
using ozgurtek.framework.common.Data;
using ozgurtek.framework.ui.controls.xamarin.Helper;
using Xamarin.Forms;

namespace ozgurtek.framework.ui.controls.xamarin.Views
{
    public class GdDoubleEntry : Entry
    {
        public GdDoubleEntry()
        {
            FontSize = 14;
            Behaviors.Add(new GdDoubleValidationBehavior());
        }

        public void AddMinMaxValueBehavior(double min, double max)
        {
            Behaviors.Add(new GdMinMaxNumberValidationBehavior(min, max));
        }

        public double? Value
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Text))
                    return null;

                return DbConvert.ToDouble(Text);
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
