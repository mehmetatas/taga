using System;

namespace Taga.Core.IoC
{
    public interface IServiceProvider
    {
        IServiceProvider Register(Type serviceType, Type classType, object singleton = null);

        object GetOrCreate(Type serviceType);
    }

    public static class ServiceProviderExtensions
    {
        public static IServiceProvider Register<TInterface, TClass>(this IServiceProvider prov)
            where TClass : class, TInterface
        {
            return prov.Register(typeof(TInterface), typeof(TClass));
        }

        public static IServiceProvider RegisterSingleton<TInterface>(this IServiceProvider prov, TInterface singleton)
        {
            return prov.Register(typeof(TInterface), singleton.GetType(), singleton);
        }

        public static TInterface GetOrCreate<TInterface>(this IServiceProvider prov)
        {
            return (TInterface)prov.GetOrCreate(typeof(TInterface));
        }
    }
}