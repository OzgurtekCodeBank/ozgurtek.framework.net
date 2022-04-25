using ozgurtek.framework.test.xamarin.Managers;
using ozgurtek.framework.ui.controls.xamarin.Pages;
using ozgurtek.framework.ui.controls.xamarin.Views;
using Xamarin.Forms;

namespace ozgurtek.framework.test.xamarin.Pages.Map
{
    public class GdMapMenuPage : GdPage
    {
        private readonly GdViewBox _viewBox;

        public GdMapMenuPage()
        {
            StackLayout stackLayout = new StackLayout();

            _viewBox = new GdViewBox();
            stackLayout.Children.Add(_viewBox);
            DialogContent.Content = stackLayout;

            this.ShowTopbar("Map");

            Init();
        }

        private void Init()
        {
            GdLabel mapLabel = new GdLabel();
            mapLabel.Text = "Click to see map samples...";
            _viewBox.AddItem("Map Samples", mapLabel);
            //mapLabel.Gesture.Tapped += MapSamplesGestureOnTapped;

        }
    }
}
