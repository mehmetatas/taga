using System;
using Ninject;
using Ninject.Web.Common;
using Taga.Core.DI;
using Taga.Core.DI.Base;

namespace Taga.Impl.DI.Ninject
{
    public class NinjectObjectFactory : ObjectFactory
    {
        private static readonly object LockObj = new Object();

        private readonly IKernel _kernel;

        public NinjectObjectFactory()
        {
            lock (LockObj)
                _kernel = new StandardKernel();
        }

        public override void Bind(Type t1, Type t2, BindingScope scope = BindingScope.Default)
        {
            var res = _kernel.Bind(t1).To(t2);
            if (scope == BindingScope.Thread)
                res.InThreadScope();
            else if (scope == BindingScope.WebRequest)
                res.InRequestScope();
            else if (scope == BindingScope.Singleton)
                res.InSingletonScope();
        }

        public override object Get(Type type)
        {
            lock (LockObj)
                return _kernel.Get(type);
        }
    }
}
