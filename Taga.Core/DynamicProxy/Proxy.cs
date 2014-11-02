using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Taga.Core.DynamicProxy
{
    public class Proxy
    {
        #region static

        private static readonly AssemblyBuilder AssemblyBuilder;
        private static readonly ModuleBuilder ModuleBuilder;

        private static readonly Dictionary<string, Type> TypeCache = new Dictionary<string, Type>();

        static Proxy()
        {
            //The type initializer for threw an exception.Request for the permission of type 'System.Security.Permissions.FileIOPermission, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089' failed


            AssemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(
                new AssemblyName("Taga.Core.DynamicProxy.Proxies"),
                AssemblyBuilderAccess.Run);

            var assemblyName = AssemblyBuilder.GetName().Name;

            ModuleBuilder = AssemblyBuilder.DefineDynamicModule(assemblyName);
        }

        private static Type GetImplementedType(Type baseType, IEnumerable<Type> interfaceTypes)
        {
            var key = GetTypeKey(baseType, interfaceTypes);
            return TypeCache.ContainsKey(key) ? TypeCache[key] : null;
        }

        private static void AddImplementation(Type baseType, IEnumerable<Type> interfaceTypes, Type implementationType)
        {
            var key = GetTypeKey(baseType, interfaceTypes);
            TypeCache.Add(key, implementationType);
        }

        private static string GetTypeKey(Type baseType, IEnumerable<Type> interfaceTypes)
        {
            var key = "__" + baseType.FullName;
            return interfaceTypes.Aggregate(key,
                (current, interfaceType) => current + "__" + interfaceType.FullName + "__");
        }

        public static Type TypeOf<TBase>(params Type[] interfaceTypes) where TBase : class
        {
            var builder = new Proxy(typeof (TBase), interfaceTypes);
            return builder.BuildProxyType();
        }

        public static TBase Of<TBase>(ICallHandler callHandler, params Type[] interfaceTypes) where TBase : class
        {
            var type = TypeOf<TBase>(interfaceTypes);
            return (TBase) Activator.CreateInstance(type, callHandler);
        }

        public static object Of(ICallHandler callHandler, Type[] interfaceTypes)
        {
            if (interfaceTypes == null || interfaceTypes.Length == 0)
                throw new ArgumentException("No interface type specified");
            return Of<object>(callHandler, interfaceTypes);
        }

        #endregion

        private readonly Type _baseClassType;
        private readonly Type[] _interfaceTypes;
        private FieldBuilder _callHandlerFieldBuilder;
        private TypeBuilder _typeBuilder;

        private string _typeName;

        private Proxy(Type baseClassType, Type[] interfaceTypes)
        {
            if (interfaceTypes == null || !interfaceTypes.Any())
                _interfaceTypes = Type.EmptyTypes;
            else if (interfaceTypes.Any(it => !it.IsInterface || !it.IsPublic || it.IsGenericType))
                throw new InvalidOperationException("Interface Types must be public and non generic");
            else
                _interfaceTypes = interfaceTypes;

            if (baseClassType == null)
                _baseClassType = typeof (object);
            else if (!baseClassType.IsClass || baseClassType.IsAbstract || baseClassType.IsGenericType ||
                     baseClassType.IsSealed || !baseClassType.IsPublic || !baseClassType.HasDefaultConstructor())
                throw new InvalidOperationException(
                    "Base Class Type must be a public, non-sealed, non-abstract, non-generic class with a public default constructor");
            else
                _baseClassType = baseClassType;
        }

        private string TypeName
        {
            get { return _typeName ?? (_typeName = BuildTypeName()); }
        }

        private string BuildTypeName()
        {
            var typeName = "__";

            if (_baseClassType != null)
                typeName += _baseClassType.Name + "__";

            typeName = _interfaceTypes.Aggregate(typeName,
                (current, interfaceType) => current + (interfaceType.Name + "__"));

            return typeName + "Proxy__";
        }

        private Type BuildType()
        {
            InitTypeBuilder();
            DefineCallHandlerField();
            BuildConstructor();
            OverrideBase();
            ImplementInterfaces();

            return _typeBuilder.CreateType();
        }

        private Type BuildProxyType()
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
            _callHandlerFieldBuilder = _typeBuilder.DefineField("_callHandler", typeof (ICallHandler),
                FieldAttributes.Private);
        }

        private void BuildConstructor()
        {
            var constructorBuilder = DeclareContsructor(); // public ProxyClass(ICallHandler callHandler)
            ImplementConstructor(constructorBuilder); // : base() { this._callHandler = callHandler; }
        }

        private void OverrideBase()
        {
            var methodsToOverride = _baseClassType.GetVirtualMethods();
            if (!_baseClassType.HasAttribute<InterceptAttribute>())
                methodsToOverride = methodsToOverride.Where(mi => mi.HasAttribute<InterceptAttribute>()).ToArray();

            foreach (var mi in methodsToOverride)
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
                new[] {typeof (ICallHandler)});
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
            il.Emit(OpCodes.Stfld, _callHandlerFieldBuilder);
            // this._callHandler = callHandler, pop this, pop callhandler argument

            il.Emit(OpCodes.Ret); // exit ctor
        }

        private void BuildMethod(MethodInfo mi)
        {
            var methodBuilder = CallHandlerMethodBuilder.GetInstance(_typeBuilder, mi,
                _callHandlerFieldBuilder);
            methodBuilder.Build();
        }
    }
}