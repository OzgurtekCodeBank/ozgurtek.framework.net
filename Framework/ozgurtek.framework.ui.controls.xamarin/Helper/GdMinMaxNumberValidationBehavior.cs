using Xamarin.Forms;

namespace ozgurtek.framework.ui.controls.xamarin.Helper
{
    public class GdMinMaxNumberValidationBehavior : Behavior<Entry>
    {
        private readonly double _min;
        private readonly double _max;

        public GdMinMaxNumberValidationBehavior(double min, double max)
        {
            _min = min;
            _max = max;
        }

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

            if (!double.TryParse(args.NewTextValue, out double newVal))
                return;

            if (newVal >= _min && newVal <= _max)
                return;

            if (newVal <= _min)
                ((Entry)sender).Text = _min.ToString();

            if (newVal >= _max)
                ((Entry)sender).Text = _max.ToString();
        }
    }
}
