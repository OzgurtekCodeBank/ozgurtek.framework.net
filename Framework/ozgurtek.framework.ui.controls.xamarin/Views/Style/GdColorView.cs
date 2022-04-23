using ozgurtek.framework.core.Data;
using Rg.Plugins.Popup.Services;
using System;
using System.Linq;
using ozgurtek.framework.ui.controls.xamarin.Pages;
using Xamarin.Forms;

namespace ozgurtek.framework.ui.controls.xamarin.Views.Style
{
    public class GdColorView : StackLayout
    {
        private GdColor _color;
        private GdColorPage _page;
        private readonly Color _noColor;
        public EventHandler ColorChanged;

        public GdColorView()
        {
            _noColor = BackgroundColor;
            HorizontalOptions = LayoutOptions.FillAndExpand;
            TapGestureRecognizer tgr = new TapGestureRecognizer();
            tgr.Tapped += TgrOnTapped;
            GestureRecognizers.Add(tgr);
        }

        public GdColor SelectedColor
        {
            get
            {
                return _color;
            }
            set
            {
                _color = value;

                Children.Clear();
                if (value.A == 0)
                {
                    BackgroundColor = _noColor;
                    Label label = new Label();
                    label.Text = "NoColor";
                    Children.Add(label);
                    return;
                }

                double[] normalize = GdColor.Normalize(value);
                Color color = new Color(normalize[0], normalize[1], normalize[2], normalize[3]);
                BackgroundColor = color;

                if (ColorChanged != null)
                    ColorChanged(this, EventArgs.Empty);
            }
        }

        private async void TgrOnTapped(object sender, EventArgs e)
        {
            if (!PopupNavigation.Instance.PopupStack.Any(p => p is GdColorPage))
            {
                _page = new GdColorPage();
                //_dialog.WheelMode = WheelMode;
                _page.SelectedColor = SelectedColor;
                _page.DialogAcceptButton.Clicked += DialogAcceptButtonOnClicked;
                _page.DialogAcceptButton.Text = "Select";
                _page.DialogCancelButton.Clicked += DialogCancelButton_Clicked;
                _page.DialogCancelButton.Text = "NoColor";
                await PopupNavigation.Instance.PushAsync(_page);
            }
        }

        private async void DialogAcceptButtonOnClicked(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PopAsync();
            SelectedColor = _page.SelectedColor;
        }

        private async void DialogCancelButton_Clicked(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PopAsync();
            SelectedColor = new GdColor(0, 0, 0, 0);
        }
    }
}