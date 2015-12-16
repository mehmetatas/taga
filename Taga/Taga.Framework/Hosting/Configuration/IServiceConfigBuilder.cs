using System;

namespace Taga.Framework.Hosting.Configuration
{
    public interface IServiceConfigBuilder
    {
        IServiceConfigBuilder Register(Type serviceType);
    }

    public static class ServiceConfigBuilderExtensions
    {
        public static IServiceConfigBuilder Register<TService>(this IServiceConfigBuilder builder)
        {
            return builder.Register(typeof(TService));
        }
    }
}