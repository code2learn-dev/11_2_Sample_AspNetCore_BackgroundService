using Microsoft.Extensions.Caching.Memory;

namespace BackgroundServiceSample.WebMinimalApi.Helpers
{
    public class CacheService
    {
        private readonly IMemoryCache _memoryCache;

        public CacheService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public void SetCacheData<T>(string key, T data) => _memoryCache.Set(key, data);

        public string? GetCacheData(string key) => _memoryCache.Get(key)?.ToString();
    }
}
