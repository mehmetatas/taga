using Ninject;
using Ninject.Web.Common;
using System;
using Taga.Core.IoC;
using IServiceProvider = Taga.Core.IoC.IServiceProvider;

namespace Taga.IoC.Ninject
{
    public class NinjectServiceProvider : IServiceProvider
    {
        protected IKernel Kernel = new StandardKernel();

        public IServiceProvider Register(Type serviceType, Type classType, ServiceScope scope = ServiceScope.Transient, object singleton = null)
        {
            var bind = Kernel.Bind(serviceType);

            switch (scope)
            {
                case ServiceScope.Transient:
                    bind.To(classType).InTransientScope();
                    break;
                case ServiceScope.PerThread:
                    bind.To(classType).InThreadScope();
                    break;
                case ServiceScope.PerWebRequest:
                    bind.To(classType).InRequestScope();
                    break;
                case ServiceScope.Singleton:
                    if (singleton == null)
                    {
                        bind.To(classType).InSingletonScope();
                    }
                    else
                    {
                        if (classType != singleton.GetType())
                        {
                            throw new InvalidOperationException(
                                String.Format("Invalid singleton type. Expected: [{0}], Found: [{1}]", classType, singleton.GetType()));
                        }
                        bind.ToConstant(singleton);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException("scope");
            }

            return this;
        }

        public object GetOrCreate(Type serviceType)
        {
            return Kernel.Get(serviceType);
        }
    }
}
