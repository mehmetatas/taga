using System;

namespace Taga.Core.DI.Base
{
    public abstract class ObjectFactory : IObjectFactory
    {
        #region Singleton

        private static readonly SetOnce<IObjectFactory> Factory = new SetOnce<IObjectFactory>("Object Factory");

        public static IObjectFactory Instance
        {
            get { return Factory.Value; }
            set { Factory.Value = value; }
        }

        #endregion

        public T Get<T>() where T : class
        {
            return Get(typeof(T)) as T;
        }

        public void Bind<T1, T2>(BindingScope scope = BindingScope.Default)
            where T2 : T1
            where T1 : class
        {
            Bind(typeof(T1), typeof(T2), scope);
        }

        public abstract object Get(Type type);
        public abstract void Bind(Type t1, Type t2, BindingScope scope = BindingScope.Default);
    }
}
