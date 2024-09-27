using System.Runtime.Caching;

namespace WorldWideWeather.Common
{
    public class CacheHelper
    {
        const double DefaultCacheDurationMinutes = 60;
        static readonly object cacheLock = new object();

        public static object GetValue(string cacheKey)
        {
            return MemoryCache.Default.Get(GetFullCacheKey(cacheKey), null);
        }

        public static void AddValue(string cacheKey, object objToCache, double durationMinutes = DefaultCacheDurationMinutes)
        {
            if (objToCache == null) return;
            var fullKey = GetFullCacheKey(cacheKey);
            object cachedObj;
            lock (cacheLock)
            {
                //Check to see if anyone wrote to the cache while we where waiting our turn to write the new value.
                cachedObj = MemoryCache.Default.Get(fullKey, null);

                if (cachedObj != null)
                {
                    return;
                }

                MemoryCache.Default.Set(fullKey, objToCache, GetPolicy(durationMinutes));
            }
        }
        private static string GetFullCacheKey(string cacheKey)
        {
            return cacheKey; ;
        }

        private static CacheItemPolicy GetPolicy(double durationMinutes = DefaultCacheDurationMinutes)
        {
            return new CacheItemPolicy()
            {
                AbsoluteExpiration = new DateTimeOffset(DateTime.Now.AddMinutes(durationMinutes))
            };
        }
    }
}
