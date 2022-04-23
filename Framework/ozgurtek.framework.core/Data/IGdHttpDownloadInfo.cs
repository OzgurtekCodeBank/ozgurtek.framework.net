namespace ozgurtek.framework.core.Data
{
    public interface IGdHttpDownloadInfo
    {
        string UserAgent { get; set; }

        IGdProxy Proxy { get; set; }

        bool UseMemoryCache { get; set; }

        bool UseDiskCache { get; set; }

        string DiskCacheFolder { get; set; }

        int HttpConnectTimeOut { get; set; }

        int HttpReadTimeOut { get; set; }

        string RefererUrl { get; set; }
    }
}