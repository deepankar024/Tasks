using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;

namespace FeedbackFormWebApp.Caching
{
    public class MemoryCacheService : ICacheService
    {
        private readonly ObjectCache _cache = MemoryCache.Default;

        public T Get<T>(string key) where T : class
        {
            return _cache[key] as T;
        }

        public void Set(string key, object value, TimeSpan expiration)
        {
            if (value == null)
            {
                return;
            }

            var policy = new CacheItemPolicy
            {
                AbsoluteExpiration = DateTimeOffset.Now.Add(expiration)
            };

            _cache.Set(key, value, policy);
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }

        public void RemoveByPrefix(string prefix)
        {
            var keysToRemove = _cache
                .Where(kvp => kvp.Key.StartsWith(prefix))
                .Select(kvp => kvp.Key)
                .ToList();

            foreach (var key in keysToRemove)
            {
                _cache.Remove(key);
            }
        }
    }
}