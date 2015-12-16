using System;
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;

namespace Taga.Framework.IoC.Castle
{
    public class CastleDependencyContainer : IDependencyContainer, IDisposable
    {
        private readonly IKernel _kernel = new DefaultKernel();

        public IDependencyContainer Register(Type serviceType, Type classType, DependencyScope scope = DependencyScope.Transient, object singleton = null)
        {
            ComponentRegistration<object> registration;

            switch (scope)
            {
                case DependencyScope.Singleton:
                    registration = singleton == null
                        ? Component.For(serviceType).ImplementedBy(classType).LifestyleSingleton()
                        : Component.For(serviceType).Instance(singleton).LifestyleSingleton();
                    break;
                case DependencyScope.PerThread:
                    registration = Component.For(serviceType).ImplementedBy(classType).LifestylePerThread();
                    break;
                case DependencyScope.PerWebRequest:
                    registration = Component.For(serviceType).ImplementedBy(classType).LifestylePerWebRequest();
                    break;
                case DependencyScope.Transient:
                    registration = Component.For(serviceType).ImplementedBy(classType).LifestyleTransient();
                    break;
                default:
                    throw new NotSupportedException("Unsupported dependency scope: " + scope);
            }

            _kernel.Register(registration.Named(Guid.NewGuid() + " [" + serviceType + "]"));
            return this;
        }

        public IDependencyContainer RegisterFactory<TClass>(Type serviceType, Func<TClass> factory, DependencyScope scope = DependencyScope.Transient)
        {
            var registration = Component.For(serviceType).UsingFactoryMethod(factory);

            switch (scope)
            {
                case DependencyScope.Singleton:
                    registration = registration.LifestyleSingleton();
                    break;
                case DependencyScope.PerThread:
                    registration = registration.LifestylePerThread();
                    break;
                case DependencyScope.PerWebRequest:
                    registration = registration.LifestylePerWebRequest();
                    break;
                case DependencyScope.Transient:
                    registration = registration.LifestyleTransient();
                    break;
            }

            _kernel.Register(registration);
            return this;

        }

        public object Resolve(Type serviceType)
        {
            return _kernel.Resolve(serviceType);
        }

        public void Dispose()
        {
            _kernel.Dispose();
        }
    }
}
