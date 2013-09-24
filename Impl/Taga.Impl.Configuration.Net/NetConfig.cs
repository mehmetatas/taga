using System;
using Taga.Core.Configuration;

namespace Taga.Impl.Configuration.Net
{
    class NetConfig : IConfig
    {
        public T Get<T>(string key)
        {
            throw new NotImplementedException();
        }

        public void Load(IConfigSource source)
        {
            throw new NotImplementedException();
        }
    }
}
