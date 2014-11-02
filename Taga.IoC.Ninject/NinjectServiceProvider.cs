using Ninject;
using System;
using Taga.Core.IoC;
using IServiceProvider = Taga.Core.IoC.IServiceProvider;

namespace Taga.IoC.Ninject
{
    public class NinjectServiceProvider : IServiceProvider
    {
        protected IKernel Kernel = new StandardKernel();

        public IServiceProvider Register(Type serviceType, Type classType, object singleton = null)
        {
            var bind = Kernel.Bind(serviceType);

            if (singleton == null)
            {
                bind.To(classType);
            }
            else
            {
                if (singleton.GetType() != classType)
                {
                    throw new InvalidSingletonTypeException(classType, singleton.GetType());
                }

                bind.ToConstant(singleton);
            }

            return this;
        }

        public object GetOrCreate(Type serviceType)
        {
            return Kernel.Get(serviceType);
        }
    }
}
