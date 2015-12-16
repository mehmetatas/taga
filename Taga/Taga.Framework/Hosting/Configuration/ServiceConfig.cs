using System.Collections.Generic;

namespace Taga.Framework.Hosting.Configuration
{
    public class ServiceConfig
    {
        public static ServiceConfig Current { get; private set; }

        public ServiceConfig()
        {
            ServiceMappings = new List<ServiceMapping>();
            Current = this;
        }

        public List<ServiceMapping> ServiceMappings { get; }
    }
}