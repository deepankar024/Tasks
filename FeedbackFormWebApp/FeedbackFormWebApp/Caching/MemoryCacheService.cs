using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;

namespace FeedbackFormWebApp.Caching
{
    public class MemoryCacheService : ICacheService
    {
        private readonly ObjectCache _cache = MemoryCache.Default; //Internal cacherefrence

        public T Get<T>(string key) where T : class          //generic type parameter, restricts T to reference types only
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

            _cache.Set(key, value, policy);  //calls 4
        }

        public void Remove(string key)
        {
            _cache.Remove(key); //by key remove cacheentry
        }

        public void RemoveByPrefix(string prefix)  //remove all key starting by prefix mtlb page2
        {
            var keysToRemove = _cache        //in-memory cache (an ObjectCache
                .Where(kvp => kvp.Key.StartsWith(prefix))       //key starts with Feedback_Page2_Size5
                .Select(kvp => kvp.Key)           //selct only the key string
                .ToList();               //List<string>) of keys

            foreach (var key in keysToRemove)
            {
                _cache.Remove(key);
            }
        }
    }
}