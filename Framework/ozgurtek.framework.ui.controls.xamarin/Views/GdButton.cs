using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace ozgurtek.framework.ui.controls.xamarin.Views
{
    public class GdButton : StackLayout
    {
        public TapGestureRecognizer Gesture;
        private GdLabel _label;
        public GdButton()
        {
            BackgroundColor = Color.White;
            _label = new GdLabel();
            _label.TextColor = Color.Black;
            _label.HorizontalOptions = LayoutOptions.Center;
            _label.HorizontalTextAlignment = TextAlignment.Center;
            _label.VerticalOptions = LayoutOptions.Center;
            _label.VerticalTextAlignment = TextAlignment.Center;
            Children.Add(_label);
            Gesture = new TapGestureRecognizer();
            GestureRecognizers.Add(Gesture);
            _label.GestureRecognizers.Add(Gesture);
        }

        public GdLabel Label => _label;


    }
}
