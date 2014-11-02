using System;

namespace Taga.Core.DI
{
    public interface IObjectFactory
    {
        object Get(Type type);
        T Get<T>() where T : class;
        void Bind(Type t1, Type t2, BindingScope scope = BindingScope.Default);
        void Bind<T1, T2>(BindingScope scope = BindingScope.Default)
            where T2 : T1
            where T1 : class;
    }
}
