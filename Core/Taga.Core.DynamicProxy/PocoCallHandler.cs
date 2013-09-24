using System;
using System.Collections.Generic;
using System.Reflection;

namespace Taga.Core.DynamicProxy
{
    public class PocoCallHandler : ICallHandler
    {
        private readonly IDictionary<string, object> _values = new SortedDictionary<string, object>();

        public object BeforeMethodCall(object obj, MethodInfo mi, object[] args)
        {
            if (mi.Name.Length < 5)
                return null;

            var methodPrefix = mi.Name.Substring(0, 4);

            if (methodPrefix == "get_")
                return Get(GetPropName(mi));

            if (methodPrefix == "set_")
                Set(GetPropName(mi), args[0]);

            return null;
        }

        public void AfterMethodCall(object obj, MethodInfo mi, object[] args, object returnValue)
        {

        }

        public void OnError(object obj, MethodInfo mi, object[] args, Exception exception)
        {
            throw new ApplicationException("An error occured during property access. See inner exception for details", exception);
        }

        private object Get(string propName)
        {
            lock (_values)
            {
                return _values.ContainsKey(propName)
                    ? _values[propName]
                    : null;
            }
        }

        private void Set(string propName, object value)
        {
            lock (_values)
            {
                if (_values.ContainsKey(propName))
                    _values[propName] = value;
                else
                    _values.Add(propName, value);
            }
        }

        private static string GetPropName(MethodInfo mi)
        {
            return mi.Name.Substring(4);
        }
    }
}
