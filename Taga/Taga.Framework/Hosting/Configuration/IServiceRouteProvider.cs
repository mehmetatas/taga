using System;
using System.Reflection;

namespace Taga.Framework.Hosting.Configuration
{
    public interface IServiceRouteProvider
    {
        string GetServiceRouteName(Type type);

        HttpMethod GetHttpMethod(MethodInfo method);

        string GetMethodRoute(MethodInfo method);

        bool IsNoAuth(MethodInfo method);
    }
}