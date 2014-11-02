using System.Collections.Generic;
using System.Configuration;
using Taga.Core.Configuration;

namespace Taga.Impl.Configuration.Net
{
    public class NetConfigSource : IConfigSource
    {
        public IDictionary<string, string> GetAll()
        {
            var dic = new Dictionary<string, string>();
            var appSettings = ConfigurationManager.AppSettings;
            var keys = appSettings.AllKeys;

            foreach (var key in keys)
                if (!dic.ContainsKey(key))
                    dic.Add(key, appSettings[key]);

            return dic;
        }
    }
}
