using Microsoft.ApplicationServer.Caching;
using Taga.Core.Cache;
using Taga.Core.Cache.Base;

namespace Taga.Impl.Cache.AppFabric
{
    public class AppFabricCacheManager : CacheManager
    {
        private readonly DataCacheFactory _factory;

        public AppFabricCacheManager()
        {
            _factory = new DataCacheFactory(); 
            SetDefaultCache(new AppFabricCache("Default", _factory.GetDefaultCache()));
        }

        protected override ICache CreateCache(string name)
        {
            var appFabricCache = _factory.GetCache(name);
            if (appFabricCache == null)
                return NullCache.Instance;
            return new AppFabricCache(name, appFabricCache);
        }

        public override void Dispose()
        {
            _factory.Dispose();
            base.Dispose();
        }
    }
}
