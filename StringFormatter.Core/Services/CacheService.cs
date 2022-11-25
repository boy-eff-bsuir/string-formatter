using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StringFormatter.Core.Interfaces;
using StringFormatter.Core.Models;

namespace StringFormatter.Core.Services
{
    public class CacheService : ICacheService
    {
        private static ConcurrentDictionary<DictionaryKey, Delegate> _cache = new();
        
        public void Clear()
        {
            _cache.Clear();
        }

        public bool TryAdd(DictionaryKey key, Delegate del)
        {
            return _cache.TryAdd(key, del);
        }

        public bool TryGetValue(DictionaryKey key, out Delegate del)
        {
            return _cache.TryGetValue(key, out del);
        }
    }
}