using System;
using System.Reflection;
using Taga.Framework.Json;
using Taga.Framework.Utils;

namespace Taga.Framework.Hosting.Impl
{
    public class ParameterResolver : IParameterResolver
    {
        private readonly IJsonSerializer _json;

        public ParameterResolver(IJsonSerializer json)
        {
            _json = json;
        }

        public void Resolve(RouteContext ctx)
        {
            ctx.Parameters = ResolveParameters(ctx);
        }

        private object[] ResolveParameters(RouteContext routeContext)
        {
            var parameters = routeContext.Method.Method.GetParameters();

            if (parameters.Length == 0)
            {
                return new object[0];
            }

            var isSingleComplexTypedParameter = parameters.Length == 1 &&
                parameters[0].ParameterType.IsClass &&
                parameters[0].ParameterType != typeof(string);

            if (isSingleComplexTypedParameter)
            {
                return ResolveComplexTypedParameter(routeContext, parameters[0].ParameterType);
            }

            if (parameters.Length == 1 && routeContext.DefaultParameter != null)
            {
                return ResolvePrimitiveTypedParameterFromDefaultParameter(routeContext.DefaultParameter, parameters);
            }

            return ResolvePrimitiveTypedParametersFromQueryString(routeContext, parameters);
        }

        private object[] ResolveComplexTypedParameter(RouteContext routeContext, Type complexParamType)
        {
            var paramValue = string.IsNullOrWhiteSpace(routeContext.HttpRequest.Content)
                ? ResolveComplexTypedParameterFromQueryString(routeContext, complexParamType)
                : ResolveComplexTypedParameterFromRequestBody(routeContext.HttpRequest.Content, complexParamType);

            return new[] { paramValue };
        }

        private object ResolveComplexTypedParameterFromQueryString(RouteContext routeContext, Type complexParamType)
        {
            var value = Activator.CreateInstance(complexParamType);

            foreach (var propInf in complexParamType.GetProperties())
            {
                var queryParamValue = routeContext.HttpRequest.GetParam(propInf.Name);

                if (string.IsNullOrWhiteSpace(queryParamValue))
                {
                    continue;
                }

                propInf.SetValue(value, SafeParse(queryParamValue, propInf.PropertyType));
            }

            return value;
        }

        private object ResolveComplexTypedParameterFromRequestBody(string requestBody, Type complexParamType)
        {
            return _json.Deserialize(requestBody, complexParamType);
        }

        private object[] ResolvePrimitiveTypedParameterFromDefaultParameter(string defaultParam, ParameterInfo[] parameters)
        {
            return new[] { SafeParse(defaultParam, parameters[0].ParameterType) };
        }

        private object[] ResolvePrimitiveTypedParametersFromQueryString(RouteContext routeContext, ParameterInfo[] parameters)
        {
            var parameterValues = new object[parameters.Length];

            foreach (var parameter in parameters)
            {
                var queryParamValue = routeContext.HttpRequest.GetParam(parameter.Name);

                if (string.IsNullOrWhiteSpace(queryParamValue))
                {
                    parameterValues[parameter.Position] = parameter.ParameterType.IsValueType
                        ? Activator.CreateInstance(parameter.ParameterType)
                        : null;
                }
                else
                {
                    parameterValues[parameter.Position] = SafeParse(queryParamValue, parameter.ParameterType);
                }
            }

            return parameterValues;
        }

        private object SafeParse(string value, Type targetType)
        {
            return QueryStringUtil.Parse(value, targetType);
        }
    }
}
