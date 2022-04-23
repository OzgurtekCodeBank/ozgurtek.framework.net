using System.Linq;
using Xamarin.Forms;

namespace ozgurtek.framework.ui.controls.xamarin.Helper
{
    public class GdIntegerValidationBehavior : Behavior<Entry>
    {
        protected override void OnAttachedTo(Entry entry)
        {
            entry.TextChanged += OnEntryTextChanged;
            base.OnAttachedTo(entry);
        }

        protected override void OnDetachingFrom(Entry entry)
        {
            entry.TextChanged -= OnEntryTextChanged;
            base.OnDetachingFrom(entry);
        }

        private static void OnEntryTextChanged(object sender, TextChangedEventArgs args)
        {
            if (!string.IsNullOrWhiteSpace(args.NewTextValue))
            {
                bool isValid = args.NewTextValue.ToCharArray().All(x => char.IsDigit(x)); //Make sure all characters are numbers

                ((Entry)sender).Text = isValid ? args.NewTextValue : args.NewTextValue.Remove(args.NewTextValue.Length - 1);
            }
        }
    }
}
