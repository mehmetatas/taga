using Taga.Core.Cache;

namespace Taga.Impl.Cache.Memory
{
    class MemoryCache : Core.Cache.Base.Cache
    {
        public MemoryCache(string name)
            : base(name)
        {
            
        }

        protected override ICacheRegion CreateRegion(string regionName)
        {
            return new MemoryCacheRegion(regionName);
        }
    }
}