using System;
using ozgurtek.framework.ui.controls.xamarin.Models;
using ozgurtek.framework.ui.controls.xamarin.Views;
using Xamarin.Forms;

namespace ozgurtek.framework.ui.controls.xamarin.Pages
{
    public class GdDoubleInputPage : GdPage
    {
        public GdDoubleEntry Entry;

        public GdDoubleInputPage()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            HasKeyboardOffset = false;//todo: Enis->Ali iyi bir çözüm değil bir bakalım...

            StackLayout entryLayout = new StackLayout()
            {
                Orientation = StackOrientation.Vertical,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Spacing = 5
            };
            Entry = new GdDoubleEntry();

            if (Device.RuntimePlatform != Device.UWP)
            {
                Entry.Focused += InputViewOnFocused;
                Entry.Unfocused += InputViewOnUnfocused;
            }

            WidthSize = new GdPageSize(200);
            HeightSize = new GdPageSize(100);
            DialogAcceptButton.IsVisible = true;
            entryLayout.Children.Add(Entry);
            DialogContent.Content = entryLayout;
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

        //protected override void OnDialogItemChanged(object sender, EventArgs e)
        //{
        //    if (!AcceptBlankEntry)
        //        DialogAcceptButton.IsEnabled = !string.IsNullOrWhiteSpace(Entry.Text);
        //}
    }
}
