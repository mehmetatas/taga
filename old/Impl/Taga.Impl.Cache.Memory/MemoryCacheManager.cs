using Taga.Core.Cache;
using Taga.Core.Cache.Base;

namespace Taga.Impl.Cache.Memory
{
    public class MemoryCacheManager : CacheManager
    {
        public MemoryCacheManager()
        {
            SetDefaultCache(CreateCache("Default"));
        }

        protected override ICache CreateCache(string name)
        {
            return new MemoryCache(name);
        }
    }
}
