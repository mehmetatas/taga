using System;
using System.Collections.Generic;
using System.Reflection;

namespace Taga.Core.DynamicProxy
{
    public class CompositeCallHandler : ICallHandler
    {
        private readonly ICollection<ICallHandler> _handlers = new List<ICallHandler>();

        public void Attach(ICallHandler handler)
        {
            lock (_handlers)
                _handlers.Add(handler);
        }

        public void Detach(ICallHandler handler)
        {
            lock (_handlers)
                _handlers.Remove(handler);
        }

        public object BeforeMethodCall(object obj, MethodInfo mi, object[] args)
        {
            lock (_handlers)
                foreach (var callHandler in _handlers)
                    callHandler.BeforeMethodCall(obj, mi, args);
            return null;
        }

        public void AfterMethodCall(object obj, MethodInfo mi, object[] args, object returnValue)
        {
            lock (_handlers)
                foreach (var callHandler in _handlers)
                    callHandler.AfterMethodCall(obj, mi, args, returnValue);
        }

        public void OnError(object obj, MethodInfo mi, object[] args, Exception exception)
        {
            lock (_handlers)
                foreach (var callHandler in _handlers)
                    callHandler.OnError(obj, mi, args, exception);
        }
    }
}
