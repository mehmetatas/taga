using System;

namespace Taga.Core.IoC
{
    public interface IServiceProvider
    {
        IServiceProvider Register(Type serviceType, Type classType, ServiceScope scope = ServiceScope.Transient, object singleton = null);

        object GetOrCreate(Type serviceType);
    }

    public static class ServiceProviderExtensions
    {
        public static IServiceProvider RegisterTransient<TInterface, TClass>(this IServiceProvider prov)
            where TClass : class, TInterface
        {
            return prov.Register(typeof(TInterface), typeof(TClass));
        }

        public static IServiceProvider RegisterPerThread<TInterface, TClass>(this IServiceProvider prov)
            where TClass : class, TInterface
        {
            return prov.Register(typeof(TInterface), typeof(TClass), ServiceScope.PerThread);
        }

        public static IServiceProvider RegisterPerWebRequest<TInterface, TClass>(this IServiceProvider prov)
            where TClass : class, TInterface
        {
            return prov.Register(typeof(TInterface), typeof(TClass), ServiceScope.PerWebRequest);
        }

        public static IServiceProvider RegisterSingleton<TInterface, TClass>(this IServiceProvider prov)
            where TClass : class, TInterface
        {
            return prov.Register(typeof(TInterface), typeof(TClass), ServiceScope.Singleton);
        }

        public static IServiceProvider RegisterSingleton<TInterface>(this IServiceProvider prov, TInterface singleton)
        {
            return prov.Register(typeof(TInterface), singleton.GetType(), ServiceScope.Singleton, singleton);
        }

        public static TInterface GetOrCreate<TInterface>(this IServiceProvider prov)
        {
            return (TInterface)prov.GetOrCreate(typeof(TInterface));
        }
    }
}