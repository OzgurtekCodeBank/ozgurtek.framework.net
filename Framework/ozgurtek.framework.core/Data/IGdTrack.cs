namespace ozgurtek.framework.core.Data
{
    public interface IGdTrack
    {
        bool CancellationPending { get; }

        void ReportProgress(double val);

        void ReportMessage(string message);
    }
}
