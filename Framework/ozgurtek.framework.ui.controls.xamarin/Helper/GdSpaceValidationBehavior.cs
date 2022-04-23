using Xamarin.Forms;

namespace ozgurtek.framework.ui.controls.xamarin.Helper
{
    public class GdSpaceValidationBehavior : Behavior<Entry>
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

        private void OnEntryTextChanged(object sender, TextChangedEventArgs args)
        {
            if (string.IsNullOrWhiteSpace(args.NewTextValue))
                return;

            string val = args.NewTextValue;
            val = val.Replace(" ", "");

            ((Entry)sender).Text = val;
        }
    }
}