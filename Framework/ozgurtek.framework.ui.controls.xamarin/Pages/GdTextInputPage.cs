using ozgurtek.framework.ui.controls.xamarin.Models;
using ozgurtek.framework.ui.controls.xamarin.Views;
using Xamarin.Forms;

namespace ozgurtek.framework.ui.controls.xamarin.Pages
{
    public class GdTextInputPage : GdPage
    {
        public GdTextEntry Entry;

        public GdTextInputPage()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            HasKeyboardOffset = false;//todo: Enis->Ali iyi bir çözüm değil bir bakalım...

            StackLayout entryLayout = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Spacing = 5
            };

            Entry = new GdTextEntry();
            DialogAcceptButton.IsVisible = true;
            Entry.TextChanged += EntryOnTextChanged;

            if (Device.RuntimePlatform != Device.UWP)
            {
                Entry.Focused += InputViewOnFocused;
                Entry.Unfocused += InputViewOnUnfocused;
            }

            WidthSize = new GdPageSize(200);
            HeightSize = new GdPageSize(100);
            DialogAcceptButton.IsEnabled = false;
            entryLayout.Children.Add(Entry);
            DialogContent.Content = entryLayout;
        }

        private void EntryOnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!AcceptBlankEntry)
                DialogAcceptButton.IsEnabled = !string.IsNullOrWhiteSpace(Entry.Text);
        }

        public bool AcceptBlankEntry { get; set; } = false;

        private void InputViewOnUnfocused(object sender, FocusEventArgs e)
        {
            TranslationY = 0;
        }

        private void InputViewOnFocused(object sender, FocusEventArgs e)
        {
            TranslationY = -50;
        }

        protected override void OnAppearingAnimationEnd()
        {
            base.OnAppearingAnimationEnd();
            Entry.Focus();
        }
    }
}
