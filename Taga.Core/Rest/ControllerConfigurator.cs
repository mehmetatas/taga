using System;

namespace Taga.Core.Rest
{
    public class ControllerConfigurator
    {
        private readonly ServiceConfig _config;

        private ServiceMapping _currentMapping;

        internal ControllerConfigurator(ServiceConfig config)
        {
            _config = config;
        }

        public ActionConfigurator<TService> ControllerFor<TService>(string controllerPath = null)
        {
            if (String.IsNullOrWhiteSpace(controllerPath))
            {
                controllerPath = typeof(TService).Name;
            }

            _currentMapping = new ServiceMapping
            {
                ServiceType = typeof(TService),
                ServiceRoute = controllerPath
            };

            _config.ServiceMappings.Add(_currentMapping);

            return new ActionConfigurator<TService>(this);
        }

        public ServiceConfig Build()
        {
            return _config;
        }

        internal void AddMethodMapping(MethodMapping methodMapping)
        {
            _currentMapping.MethodMappings.Add(methodMapping);
        }
    }
}
