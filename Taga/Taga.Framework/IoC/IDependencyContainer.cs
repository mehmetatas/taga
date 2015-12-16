using System;

namespace Taga.Framework.IoC
{
    public interface IDependencyContainer
    {
        IDependencyContainer Register(Type serviceType, Type classType, DependencyScope scope = DependencyScope.Transient, object singleton = null);

        IDependencyContainer RegisterFactory<TClass>(Type serviceType, Func<TClass> factory, DependencyScope scope = DependencyScope.Transient);

        object Resolve(Type serviceType);
    }

    public static class DependencyContainerExtensions
    {
        public static IDependencyContainer RegisterTransient<TInterface, TClass>(this IDependencyContainer prov)
            where TClass : class, TInterface
        {
            return prov.Register(typeof(TInterface), typeof(TClass));
        }

        public static IDependencyContainer RegisterPerThread<TInterface, TClass>(this IDependencyContainer prov)
            where TClass : class, TInterface
        {
            return prov.Register(typeof(TInterface), typeof(TClass), DependencyScope.PerThread);
        }

        public static IDependencyContainer RegisterPerWebRequest<TInterface, TClass>(this IDependencyContainer prov)
            where TClass : class, TInterface
        {
            return prov.Register(typeof(TInterface), typeof(TClass), DependencyScope.PerWebRequest);
        }

        public static IDependencyContainer RegisterSingleton<TInterface, TClass>(this IDependencyContainer prov)
            where TClass : class, TInterface
        {
            return prov.Register(typeof(TInterface), typeof(TClass), DependencyScope.Singleton);
        }

        public static IDependencyContainer RegisterSingleton<TInterface>(this IDependencyContainer prov, TInterface singleton)
        {
            return prov.Register(typeof(TInterface), singleton.GetType(), DependencyScope.Singleton, singleton);
        }

        public static IDependencyContainer RegisterTransient<TInterface, TClass>(this IDependencyContainer prov, Func<TClass> factory)
            where TClass : class, TInterface
        {
            return prov.RegisterFactory(typeof(TInterface), factory);
        }

        public static IDependencyContainer RegisterPerThread<TInterface, TClass>(this IDependencyContainer prov, Func<TClass> factory)
            where TClass : class, TInterface
        {
            return prov.RegisterFactory(typeof(TInterface), factory, DependencyScope.PerThread);
        }

        public static IDependencyContainer RegisterPerWebRequest<TInterface, TClass>(this IDependencyContainer prov, Func<TClass> factory)
            where TClass : class, TInterface
        {
            return prov.RegisterFactory(typeof(TInterface), factory, DependencyScope.PerWebRequest);
        }

        public static IDependencyContainer RegisterSingleton<TInterface, TClass>(this IDependencyContainer prov, Func<TClass> factory)
            where TClass : class, TInterface
        {
            return prov.RegisterFactory(typeof(TInterface), factory, DependencyScope.Singleton);
        }

        public static TInterface Resolve<TInterface>(this IDependencyContainer prov)
        {
            return (TInterface)prov.Resolve(typeof(TInterface));
        }
    }
}
