using System;
using System.Collections.Generic;

namespace FeedbackFormWebApp.Caching
{
    public interface ICacheService
    {
        T Get<T>(string key) where T : class;
        void Set(string key, object value, TimeSpan expiration);
        void Remove(string key);
        void RemoveByPrefix(string prefix);
    }
}