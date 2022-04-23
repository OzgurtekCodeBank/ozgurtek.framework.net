using ozgurtek.framework.core.Data;
using System;

namespace ozgurtek.framework.common.Data
{
    public class GdHttpDownloadInfo : IGdHttpDownloadInfo
    {
        private static readonly Random Random = new Random();
        private string _userAgent = string.Format("Mozilla/5.0 (Windows NT {1}.0; {2}rv:{0}.0) Gecko/20100101 Firefox/{0}.0",
            Random.Next(DateTime.Today.Year - 1969 - 5, DateTime.Today.Year - 1969),
            Random.Next(0, 10) % 2 == 0 ? 10 : 6,
            Random.Next(0, 10) % 2 == 1 ? string.Empty : "WOW64; ");

        private int _httpConnectTimeOut = 5000;
        private int _httpReadTimeOut = 3000;
        private string _refererUrl = string.Empty;
        private bool _useMemoryCache;
        private bool _useDiskCache;
        private string _diskCacheFolder = string.Empty;
        private IGdProxy _proxy;

        public string UserAgent
        {
            get => _userAgent;
            set => _userAgent = value;
        }

        public int HttpConnectTimeOut
        {
            get => _httpConnectTimeOut;
            set => _httpConnectTimeOut = value;
        }

        public int HttpReadTimeOut
        {
            get => _httpReadTimeOut;
            set => _httpReadTimeOut = value;
        }

        public string RefererUrl
        {
            get => _refererUrl;
            set => _refererUrl = value;
        }

        public bool UseMemoryCache
        {
            get => _useMemoryCache;
            set => _useMemoryCache = value;
        }

        public bool UseDiskCache
        {
            get => _useDiskCache;
            set => _useDiskCache = value;
        }

        public string DiskCacheFolder
        {
            get => _diskCacheFolder;
            set => _diskCacheFolder = value;
        }

        public IGdProxy Proxy
        {
            get => _proxy;
            set => _proxy = value;
        }
    }
}