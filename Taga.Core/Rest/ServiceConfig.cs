using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Taga.Core.Rest
{
    public class ServiceConfig
    {
        public ServiceConfig()
        {
            ServiceMappings = new List<ServiceMapping>();
        }

        public List<ServiceMapping> ServiceMappings { get; set; }
    }

    public class ControllerConfigurator
    {
        private readonly ServiceConfig _config = new ServiceConfig();

        private ServiceMapping _currentMapping;

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

        public ServiceConfig Configure()
        {
            return _config;
        }

        internal void AddMethodMapping(MethodMapping methodMapping)
        {
            _currentMapping.MethodMappings.Add(methodMapping);
        }
    }

    public class ActionConfigurator<TService>
    {
        private readonly ControllerConfigurator _serviceConfigurator;

        public ActionConfigurator(ControllerConfigurator serviceConfigurator)
        {
            _serviceConfigurator = serviceConfigurator;
        }

        public ActionConfigurator<TNewService> ControllerFor<TNewService>(string controllerPath = null)
        {
            return _serviceConfigurator.ControllerFor<TNewService>(controllerPath);
        }

        public ActionConfigurator<TService> ActionFor(Expression<Action<TService>> actionExpression,
            string actionPath = null, HttpMethodType httpMethod = HttpMethodType.Post)
        {
            return ActionFor((LambdaExpression)actionExpression, actionPath, httpMethod);
        }

        public ActionConfigurator<TService> ActionFor<TResponse>(Expression<Func<TService, TResponse>> actionExpression, string actionPath = null, HttpMethodType httpMethod = HttpMethodType.Post)
        {
            return ActionFor((LambdaExpression)actionExpression, actionPath, httpMethod);
        }

        private ActionConfigurator<TService> ActionFor(LambdaExpression actionExpression, string actionPath, HttpMethodType httpMethod)
        {
            var methodCallExpression = actionExpression.Body as MethodCallExpression;

            if (methodCallExpression == null)
            {
                throw new Exception("actionExpression must be MethodCallExpression!");
            }

            if (String.IsNullOrWhiteSpace(actionPath))
            {
                actionPath = methodCallExpression.Method.Name;
            }

            _serviceConfigurator.AddMethodMapping(new MethodMapping
            {
                Method = methodCallExpression.Method,
                MethodRoute = actionPath,
                HttpMethodType = httpMethod
            });

            return this;
        }

        public ServiceConfig Configure()
        {
            return _serviceConfigurator.Configure();
        }
    }
}