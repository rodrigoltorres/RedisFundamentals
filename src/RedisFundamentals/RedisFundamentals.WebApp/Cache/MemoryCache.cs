namespace RedisFundamentals.WebApp.Cache
{
    using System;
    using Microsoft.Extensions.Caching.Memory;

    public class MemoryCache : ICache
    {
        private readonly Microsoft.Extensions.Caching.Memory.MemoryCache memoryCache;

        public MemoryCache(Microsoft.Extensions.Caching.Memory.MemoryCache memoryCache)
        {
            this.memoryCache = memoryCache;
        }

        public bool Add(string key, object obj, TimeSpan? expiry = null)
        {
            if(expiry.HasValue)
                return this.memoryCache.Set(key, obj, expiry.Value) != null;
            else
                return this.memoryCache.Set(key, obj) != null;
        }

        public bool Delete(string key)
        {
            try
            {
                this.memoryCache.Remove(key);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public T Get<T>(string key)
        {
            return this.memoryCache.Get<T>(key);
        }

        public TimeSpan? KeyTimeToLive(string key)
        {
            throw new NotImplementedException();
        }
    }
}
