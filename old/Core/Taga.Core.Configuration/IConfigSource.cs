using System.Collections.Generic;

namespace Taga.Core.Configuration
{
    public interface IConfigSource
    {
        IDictionary<string, string> GetAll();
    }
}
