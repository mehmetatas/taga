using System.Collections.Generic;

namespace Taga.Orm.Dynamix
{
    public interface IProxyValues
    {
        IDictionary<string, object> Values { get; }
    }
}