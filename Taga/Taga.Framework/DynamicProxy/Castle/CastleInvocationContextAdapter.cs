using System.Linq;
using Castle.DynamicProxy;
using System;
using System.Reflection;

namespace Taga.Framework.DynamicProxy.Castle
{
    class CastleInvocationContextAdapter : IInvocationContext
    {
        private readonly IInvocation _invocation;

        public CastleInvocationContextAdapter(IInvocation invocation)
        {
            _invocation = invocation;
            ResolveServiceType(invocation);
        }

        private void ResolveServiceType(IInvocation invocation)
        {
            var serviceType = invocation.Method.DeclaringType;
            var interfaces = invocation.Proxy.GetType().GetInterfaces();
            var tmp = serviceType;
            while ((tmp = interfaces.FirstOrDefault(i => tmp != null && i.GetInterface(tmp.Name) != null)) != null)
            {
                serviceType = tmp;
            }
            ServiceType = serviceType;
        }

        public Type ServiceType { get; private set; }

        public MethodInfo Method => _invocation.Method;

        public object[] Arguments => _invocation.Arguments;

        public object ReturnValue
        {
            get { return _invocation.ReturnValue; }
            set { _invocation.ReturnValue = value; }
        }

        public void Proceed()
        {
            _invocation.Proceed();
        }
    }
}
