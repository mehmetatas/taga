using System;
using System.Collections.Generic;

namespace Taga.Core.Rest
{
    public class ServiceMapping
    {
        public ServiceMapping()
        {
            MethodMappings = new List<MethodMapping>();
        }

        public Type ServiceType { get; set; }
        public string ServiceRoute { get; set; }

        public List<MethodMapping> MethodMappings { get; set; }
    }
}