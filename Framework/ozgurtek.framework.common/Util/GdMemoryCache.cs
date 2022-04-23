using Microsoft.Extensions.Caching.Memory;

namespace ozgurtek.framework.common.Util
{
    public class GdMemoryCache
    {
        private static GdMemoryCache _instance;
        private static readonly object Padlock = new object();
        private readonly MemoryCache _cache;

        private GdMemoryCache()
        {
            _cache = new MemoryCache(new MemoryCacheOptions());
        }

        public static GdMemoryCache Instance
        {
            get
            {
                lock (Padlock)
                {
                    return _instance ?? (_instance = new GdMemoryCache());
                }
            }
        }

        public void Add(string path, byte[] image)
        {
            _cache.Set(path, image);
        }

        public byte[] Get(string path)
        {
            object obj = _cache.Get(path);
            if (obj == null)
                return null;

            return (byte[])obj;
        }
    }
}
