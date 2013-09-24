using Microsoft.ApplicationServer.Caching;
using System;
using Taga.Core.Cache;

namespace Taga.Impl.Cache.AppFabric
{
    class AppFabricCache : Core.Cache.Base.Cache
    {
        private readonly DataCache _cache;

        internal AppFabricCache(string name, DataCache cache) : base(name)
        {
            _cache = cache;
        }

        protected override ICacheRegion CreateRegion(string regionName)
        {
            lock (_cache)
            {
                if (_cache.CreateRegion(regionName))
                    return new AppFabricCacheRegion(regionName, _cache);
                throw new DataCacheException(String.Format("Unabele to create region '{0}' in cache '{1}'", regionName, Name));
            }
        }
    }
}
