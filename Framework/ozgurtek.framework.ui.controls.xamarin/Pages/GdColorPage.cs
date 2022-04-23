using System;
using ozgurtek.framework.core.Data;
using ozgurtek.framework.ui.controls.xamarin.Models;
using ozgurtek.framework.ui.controls.xamarin.Views;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;

namespace ozgurtek.framework.ui.controls.xamarin.Pages
{
    internal class GdColorPage : GdPage
    {
        private StackLayout _colorLayout;
        private Slider _alphaSlider;

        public GdColorPage()
        {
            WidthSize = new GdPageSize(0.5, 330);
            HeightSize = new GdPageSize(130);

            _colorLayout = new StackLayout()
            {
                HeightRequest = 30,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Color.Purple
            };

            TapGestureRecognizer colorTapped = new TapGestureRecognizer();
            colorTapped.Tapped += ColorTappedOnTapped;
            _colorLayout.GestureRecognizers.Add(colorTapped);


            _alphaSlider = new Slider(0, 1, 1);
            _alphaSlider.ValueChanged += AlphaSliderOnValueChanged;

            DialogTopBar.IsVisible = false;
            DialogAcceptButton.IsVisible = true;
            DialogCancelButton.IsVisible = true;
            DialogAcceptButton.HorizontalOptions = LayoutOptions.Center;

            GdViewBox viewBox = new GdViewBox();

            viewBox.AddItem("Color", _colorLayout);
            viewBox.AddItem("Alpha", _alphaSlider);

            //sLayout.Children.Add(_colorSample);
            DialogContent.Content = viewBox;
        }

        private void AlphaSliderOnValueChanged(object sender, ValueChangedEventArgs e)
        {
            Color c = _colorLayout.BackgroundColor;
            Color newColor = Color.FromRgba(c.R, c.G, c.B, e.NewValue);
            _colorLayout.BackgroundColor = newColor;
        }

        private async void ColorTappedOnTapped(object sender, EventArgs e)
        {
            GdColorSelectPage bcSelect = new GdColorSelectPage();
            bcSelect.Alpha = _alphaSlider.Value;
            //bcSelect.ListView.DisableSelectionColorChange = true;
            bcSelect.ListView.ItemClicked += (o, item) =>
            {

                _colorLayout.BackgroundColor = item.BackgroundColor;
                PopupNavigation.Instance.PopAsync();
            };
            await PopupNavigation.Instance.PushAsync(bcSelect);
        }


        public GdColor SelectedColor
        {
            get
            {
                Color color = _colorLayout.BackgroundColor;
                return GdColor.FromNormalize(color.R, color.G, color.B, color.A);
            }
            set
            {
                double[] normalize = GdColor.Normalize(value);
                if (normalize[3] != 0)
                {
                    Color color = new Color(normalize[0], normalize[1], normalize[2], normalize[3]);

                    _colorLayout.BackgroundColor = color;
                    _alphaSlider.Value = color.A;
                }
            }
        }
    }
}
