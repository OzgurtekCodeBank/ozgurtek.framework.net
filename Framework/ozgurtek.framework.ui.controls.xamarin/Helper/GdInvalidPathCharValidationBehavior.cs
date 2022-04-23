using System.IO;
using System.Text.RegularExpressions;
using Xamarin.Forms;

namespace ozgurtek.framework.ui.controls.xamarin.Helper
{
    public class GdInvalidPathCharValidationBehavior : Behavior<Entry>
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

            string regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            Regex r = new Regex($"[{Regex.Escape(regexSearch)}]");
            string newVal = r.Replace(val, "");
            if (newVal.Equals(val))
                return;

            ((Entry)sender).Text = newVal;
        }
    }
}