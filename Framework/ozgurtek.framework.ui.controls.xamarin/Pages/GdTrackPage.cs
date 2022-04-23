using System;
using System.Threading.Tasks;
using ozgurtek.framework.core.Data;

namespace ozgurtek.framework.ui.controls.xamarin.Pages
{
    public class GdTrackPage : GdProgressPage, IGdTrack
    {
        private bool _cancelationPending;
        private EventHandler<double> _progressChanged;
        private Action _performJobAction;
        public EventHandler ProgressCompleted;

        public GdTrackPage()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            DialogTopBar.IsVisible = false;

            Disappearing += (sender, args) =>
            {
                SetCancelationPendingTrue();
            };

            ProgressChanged += (sender, d) =>
            {
                Step(d);
                if (d >= 1 && ProgressCompleted != null)
                    ProgressCompleted(this, EventArgs.Empty);
            };
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            PerformJob();
        }

        public Task PerformJob()
        {
            return Task.Run(() => _performJobAction?.Invoke());
        }

        public void SetAction(Action performJobAction)
        {
            _performJobAction = performJobAction;
        }

        public bool CancellationPending
        {
            get { return _cancelationPending; }
        }

        public EventHandler<double> ProgressChanged
        {
            get { return _progressChanged; }
            set { _progressChanged = value; }
        }

        public void ReportProgress(double val)
        {
            if (_progressChanged != null)
                _progressChanged(this, val);
        }

        public void ReportMessage(string message)
        {
            
        }

        public void SetCancelationPendingTrue()
        {
            _cancelationPending = true;
        }
    }
}
