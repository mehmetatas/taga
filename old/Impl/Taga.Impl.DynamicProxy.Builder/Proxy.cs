using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Taga.Core.DynamicProxy;
using Taga.Core.Extensions;

namespace Taga.Impl.DynamicProxy.Builder
{
    internal class Proxy
    {
        #region static

        private static readonly AssemblyBuilder AssemblyBuilder;
        private static readonly ModuleBuilder ModuleBuilder;

        private static readonly object LockObj = new Object();
        private static readonly Dictionary<string, Type> TypeCache = new Dictionary<string, Type>();

        static Proxy()
        {
            lock (LockObj)
            {
                AssemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(
                    new AssemblyName("Taga.Core.DynamicProxy.Proxies"),
                    AssemblyBuilderAccess.Run);

                var assemblyName = AssemblyBuilder.GetName().Name;

                ModuleBuilder = AssemblyBuilder.DefineDynamicModule(assemblyName);
            }
        }

        private static Type GetImplementedType(Type baseType, Type[] interfaceTypes)
        {
            var key = GetTypeKey(baseType, interfaceTypes);
            return TypeCache.ContainsKey(key) ? TypeCache[key] : null;
        }

        private static void AddImplementation(Type baseType, Type[] interfaceTypes, Type implementationType)
        {
            var key = GetTypeKey(baseType, interfaceTypes);
            TypeCache.Add(key, implementationType);
        }

        private static string GetTypeKey(Type baseType, Type[] interfaceTypes)
        {
            var key = String.Empty;
            key += baseType.FullName;
            key = interfaceTypes.Aggregate(key, (current, interfaceType) => current + interfaceType);
            return key;
        }

        internal static TBase Of<TBase>(ICallHandler callHandler, params Type[] interfaceTypes) where TBase : class
        {
            var builder = new Proxy(typeof(TBase), interfaceTypes);
            var type = builder.GetProxyType();
            return (TBase)Activator.CreateInstance(type, callHandler);
        }

        internal static object Of(ICallHandler callHandler, Type[] interfaceTypes)
        {
            if (interfaceTypes == null || interfaceTypes.Length == 0)
                throw new InvalidOperationException("No interface type specified");
            return Of<object>(callHandler, interfaceTypes);
        }

        #endregion

        private TypeBuilder _typeBuilder;
        private FieldBuilder _callHandlerFieldBuilder;
        private readonly Type _baseClassType;
        private readonly Type[] _interfaceTypes;

        internal Proxy(Type baseClassType, Type[] interfaceTypes)
        {
            if (interfaceTypes == null || !interfaceTypes.Any())
                _interfaceTypes = Type.EmptyTypes;
            else if (interfaceTypes.Any(it => !it.IsInterface || !it.IsPublic || it.IsGenericType))
                throw new InvalidOperationException("Interface Types must be public and non generic");
            else
                _interfaceTypes = interfaceTypes;

            if (baseClassType == null)
                _baseClassType = typeof(object);
            else if (!baseClassType.IsClass || baseClassType.IsAbstract || baseClassType.IsGenericType || baseClassType.IsSealed || !baseClassType.IsPublic || !baseClassType.HasDefaultConstructor())
                throw new InvalidOperationException("Base Class Type must be a public, non-sealed, non-abstract, non-generic class with a public default constructor");
            else
                _baseClassType = baseClassType;
        }

        private string _typeName;
        private string TypeName
        {
            get { return _typeName ?? (_typeName = BuildTypeName()); }
        }

        internal string BuildTypeName()
        {
            var typeName = "__";

            if (_baseClassType != null)
                typeName += _baseClassType.Name + "__";

            foreach (var interfaceType in _interfaceTypes)
                typeName += interfaceType.Name + "__";

            return typeName + "Proxy__";
        }

        private Type BuildType()
        {
            InitTypeBuilder();
            DefineCallHandlerField();
            BuildConstructor();
            ExtendBase();
            ImplementInterfaces();

            return _typeBuilder.CreateType();
        }



        private Type GetProxyType()
        {
            var type = GetImplementedType(_baseClassType, _interfaceTypes);
            if (type != null)
                return type;

            type = BuildType();

            AddImplementation(_baseClassType, _interfaceTypes, type);
            return type;
        }

        private void InitTypeBuilder()
        {
            // public class __BaseClass__Interface1__Interface2__Proxy__ : BaseClass, Interface1, Interface2
            _typeBuilder = ModuleBuilder.DefineType(
                TypeName,
                TypeAttributes.Public | TypeAttributes.Class,
                _baseClassType,
                _interfaceTypes);
        }

        private void DefineCallHandlerField()
        {
            // private ICallHandler _callHandler;
            _callHandlerFieldBuilder = _typeBuilder.DefineField("_callHandler", typeof(ICallHandler), FieldAttributes.Private);
        }

        private void BuildConstructor()
        {
            var constructorBuilder = DeclareContsructor();   // public ProxyClass(ICallHandler callHandler)
            ImplementConstructor(constructorBuilder);       // : base() { this._callHandler = callHandler; }
        }

        private void ExtendBase()
        {
            foreach (var mi in _baseClassType.GetVirtualMethods())
                BuildMethod(mi);
        }

        private void ImplementInterfaces()
        {
            foreach (var methodInfo in _interfaceTypes.SelectMany(interfaceType => interfaceType.GetMethods()))
                BuildMethod(methodInfo);
        }

        private ConstructorBuilder DeclareContsructor()
        {
            var constructorBuilder = _typeBuilder.DefineConstructor(
                MethodAttributes.Public,
                CallingConventions.HasThis,
                new[] { typeof(ICallHandler) });
            return constructorBuilder;
        }

        private void ImplementConstructor(ConstructorBuilder constructorBuilder)
        {
            var baseCtor = _baseClassType.GetConstructor(Type.EmptyTypes);

            var il = constructorBuilder.GetILGenerator();

            // call base ctor
            il.Emit(OpCodes.Ldarg_0); // push this
            il.Emit(OpCodes.Call, baseCtor); // Call base constructor this.base(); pops this

            // set _callHandler
            il.Emit(OpCodes.Ldarg_0); // push this
            il.Emit(OpCodes.Ldarg_1); // push callHandler argument
            il.Emit(OpCodes.Stfld, _callHandlerFieldBuilder); // this._callHandler = callHandler, pop this, pop callhandler argument
            
            il.Emit(OpCodes.Ret); // exit ctor
        }

        private void BuildMethod(MethodInfo mi)
        {
            var methodBuilder = CallHandlerMethodBuilder.GetInstance(_typeBuilder, mi, _callHandlerFieldBuilder);
            methodBuilder.Build();
        }
    }
}