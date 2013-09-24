using Microsoft.ApplicationServer.Caching;
using System.Collections.Generic;
using System.Linq;
using Taga.Core.Cache;

namespace Taga.Impl.Cache.AppFabric
{
    class AppFabricCacheRegion : ICacheRegion
    {
        private readonly DataCache _cache;

        public string Name { get; private set; }

        internal AppFabricCacheRegion(string name, DataCache cache)
        {
            Name = name;
            _cache = cache;
        }

        public T Get<T>(string key)
        {
            lock (_cache)
            {
                var res = _cache.Get(key, Name);
                return res is T ? (T) res : default(T);
            }
        }

        public IEnumerable<T> Get<T>(params string[] tags)
        {
            lock (_cache)
            {
                var res = _cache.GetObjectsByAnyTag(GetTags(tags), Name);
                return res.Select(kv => kv.Value).OfType<T>();
            }
        }

        public void Add<T>(string key, T item, params string[] tags)
        {
            lock (_cache)
                _cache.Add(key, item, GetTags(tags), Name);
        }

        public void Put<T>(string key, T item, params string[] tags)
        {
            lock (_cache)
                _cache.Put(key, item, GetTags(tags), Name);
        }

        public void Clear()
        {
            lock (_cache)
                _cache.ClearRegion(Name);
        }

        protected static IEnumerable<DataCacheTag> GetTags(IEnumerable<string> tags)
        {
            return tags.Select(tag => new DataCacheTag(tag));
        }

        public void Dispose()
        {
            Clear();
        }
    }
}
