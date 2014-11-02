using System;
using System.Linq;
using System.Collections.Generic;
using Taga.Core.Cache;

namespace Taga.Impl.Cache.Memory
{
    class MemoryCacheRegion : ICacheRegion
    {
        private readonly object _lockObj = new Object();

        private readonly IDictionary<string, object> _cache;
        private readonly IDictionary<string, List<string>> _tagCache;

        public string Name { get; private set; }

        internal MemoryCacheRegion(string name)
        {
            Name = name;

            _cache = new SortedDictionary<string, object>();
            _tagCache = new SortedDictionary<string, List<string>>();
        }

        public T Get<T>(string key)
        {
            lock (_lockObj)
            {
                if (_cache.ContainsKey(key))
                    return (T)_cache[key];
                return default(T);
            }
        }

        public IEnumerable<T> Get<T>(params string[] tags)
        {
            if (tags == null || tags.Length == 0)
                return new T[0];

            lock (_lockObj)
            {
                var keys = _tagCache.Where(kv => tags.Contains(kv.Key)).SelectMany(kv => kv.Value).Distinct();
                return _cache.Where(kv => keys.Contains(kv.Key)).Select(kv => kv.Value).OfType<T>();
            }
        }

        public void Add<T>(string key, T item, params string[] tags)
        {
            lock (_lockObj)
            {
                if (_cache.ContainsKey(key))
                    throw new InvalidOperationException("Cache already contains " + key);

                _cache.Add(key, item);

                AddTags<T>(key, tags);
            }
        }

        public void Put<T>(string key, T item, params string[] tags)
        {
            lock (_lockObj)
            {
                if (_cache.ContainsKey(key))
                    _cache[key] = item;
                else
                    _cache.Add(key, item);

                AddTags<T>(key, tags);
            }
        }

        public void Clear()
        {
            lock (_lockObj)
            {
                _cache.Clear();
                _tagCache.Clear();
            }
        }

        private void AddTags<T>(string key, string[] tags)
        {
            if (tags == null || tags.Length == 0)
                return;

            foreach (var tag in tags)
            {
                if (!_tagCache.ContainsKey(tag))
                    _tagCache.Add(tag, new List<string>());

                if (!_tagCache[tag].Contains(key))
                    _tagCache[tag].Add(key);
            }
        }

        public void Dispose()
        {
            Clear();
        }
    }
}
