using ozgurtek.framework.core.Data;
using System;

namespace ozgurtek.framework.common.Util
{
    public class GdTrack : IGdTrack
    {
        private EventHandler<double> _progressChanged;

        private bool _cancelationPending;

        public bool CancellationPending
        {
            get { return _cancelationPending; }
        }

        public EventHandler<double> ProgressChanged
        {
            get { return _progressChanged; }
            set => _progressChanged = value;
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
