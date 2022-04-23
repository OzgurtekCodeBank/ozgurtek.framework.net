using System;
using ozgurtek.framework.ui.controls.xamarin.Models;
using Xamarin.Forms;

namespace ozgurtek.framework.ui.controls.xamarin.Pages
{
    public class GdProgressPage : GdPage
    {
        private ProgressBar _progressBar;
        private double _stepThreshold = 0.1;
        private double _step;

        public GdProgressPage()
        {
            InitalizeComponents();
            _step = _stepThreshold;
        }

        private void InitalizeComponents()
        {
            WidthSize = new GdPageSize(0.2, 330);
            HeightSize = new GdPageSize(30);

            StackLayout layout = new StackLayout()
            {
                Orientation = StackOrientation.Vertical
            };

            _progressBar = new ProgressBar()
            {
                Progress = 0,
                ProgressColor = Color.Red
            };
            layout.Children.Add(_progressBar);

            DialogContent.Content = layout;
        }

        public void Step(double d)
        {
            if (d > _step)
            {
                _progressBar.ProgressTo(d, 10, Easing.Linear);
                _step += _stepThreshold;
            }
        }

        public double StepThreshold
        {
            get => _stepThreshold;
            set => _step = _stepThreshold = value;
        }
    }
}
