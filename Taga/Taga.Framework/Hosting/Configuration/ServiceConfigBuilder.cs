using System;
using System.Linq;

namespace Taga.Framework.Hosting.Configuration
{
    public class ServiceConfigBuilder : IServiceConfigBuilder
    {
        private readonly IServiceRouteProvider _provider;
        private readonly ServiceConfig _config;

        public ServiceConfigBuilder(IServiceRouteProvider provider)
        {
            _provider = provider;
            _config = new ServiceConfig();
        }

        public IServiceConfigBuilder Register(Type serviceType)
        {
            _config.ServiceMappings.Add(new ServiceMapping
            {
                ServiceType = serviceType,
                ServiceRoute = _provider.GetServiceRouteName(serviceType),
                MethodMappings = serviceType.GetMethods().Select(method => new MethodMapping
                {
                    Method = method,
                    HttpMethod = _provider.GetHttpMethod(method),
                    MethodRoute = _provider.GetMethodRoute(method),
                    NoAuth = _provider.IsNoAuth(method)
                }).ToList()
            });

            return this;
        }
    }
}