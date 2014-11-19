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

        private static readonly ModuleBuilder ModuleBuilder;

        private static readonly Dictionary<string, Type> TypeCache = new Dictionary<string, Type>();

        static Proxy()
        {
            // The type initializer for threw an exception.
            // Request for the permission of type 'System.Security.Permissions.FileIOPermission, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089' failed
            var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(
                new AssemblyName("Taga.Core.DynamicProxy.Proxies"),
                AssemblyBuilderAccess.Run);

            var assemblyName = assemblyBuilder.GetName().Name;

            ModuleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName);
        }

        private static Type GetImplementedType(Type baseType, IEnumerable<Type> interfaceTypes, Type interceptorType)
        {
            var key = GetTypeKey(baseType, interfaceTypes, interceptorType);
            return TypeCache.ContainsKey(key) ? TypeCache[key] : null;
        }

        private static void AddImplementation(Type baseType, IEnumerable<Type> interfaceTypes, Type implementationType, Type interceptorType)
        {
            var key = GetTypeKey(baseType, interfaceTypes, interceptorType);
            TypeCache.Add(key, implementationType);
        }

        private static string GetTypeKey(Type baseType, IEnumerable<Type> interfaceTypes, Type interceptorType)
        {
            var key = "__" + baseType.FullName + "__" + interceptorType.FullName + "__";
            return interfaceTypes.Aggregate(key,
                (current, interfaceType) => current + "__" + interfaceType.FullName + "__");
        }

        public static Type TypeOf<TBase>(params Type[] interfaceTypes) where TBase : class
        {
            var builder = new Proxy(typeof(TBase), interfaceTypes, typeof(IMethodCallInterceptor));
            return builder.BuildProxyType();
        }

        public static Type TypeOf<TBase, TInterceptor>(params Type[] interfaceTypes)
            where TBase : class
            where TInterceptor : class, IMethodCallInterceptor
        {
            var builder = new Proxy(typeof(TBase), interfaceTypes, typeof(TInterceptor));
            return builder.BuildProxyType();
        }

        public static Type TypeOf<TBase>(Type interceptorType, params Type[] interfaceTypes)
            where TBase : class
        {
            var builder = new Proxy(typeof(TBase), interfaceTypes, interceptorType);
            return builder.BuildProxyType();
        }

        public static TBase Of<TBase>(IMethodCallInterceptor interceptor, params Type[] interfaceTypes) where TBase : class
        {
            var type = TypeOf<TBase>(interceptor.GetType(), interfaceTypes);
            return (TBase)Activator.CreateInstance(type, interceptor);
        }

        public static object Of(IMethodCallInterceptor interceptor, Type[] interfaceTypes)
        {
            if (interfaceTypes == null || interfaceTypes.Length == 0)
                throw new ArgumentException("No interface type specified");
            return Of<object>(interceptor, interfaceTypes);
        }

        #endregion

        private readonly Type _baseClassType;
        private readonly Type[] _interfaceTypes;
        private readonly Type _interceptorType;
        private FieldBuilder _interceptorFieldBuilder;
        private TypeBuilder _typeBuilder;

        private string _typeName;

        private Proxy(Type baseClassType, Type[] interfaceTypes, Type interceptorType)
        {
            if (!typeof(IMethodCallInterceptor).IsAssignableFrom(interceptorType))
                throw new InvalidOperationException("Call handler must implement " + typeof(IMethodCallInterceptor));

            _interceptorType = interceptorType;

            if (interfaceTypes == null || !interfaceTypes.Any())
                _interfaceTypes = Type.EmptyTypes;
            else if (interfaceTypes.Any(it => !it.IsInterface || !it.IsPublic || it.IsGenericType))
                throw new InvalidOperationException("Interface Types must be public and non generic");
            else
                _interfaceTypes = interfaceTypes;

            if (baseClassType == null)
                _baseClassType = typeof(object);
            else if (!baseClassType.IsClass || baseClassType.IsAbstract || baseClassType.IsGenericType ||
                     baseClassType.IsSealed || !baseClassType.IsPublic) // || !baseClassType.HasDefaultConstructor()
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
            DefineInterceptorField();
            BuildConstructor();
            OverrideBase();
            ImplementInterfaces();

            return _typeBuilder.CreateType();
        }

        private Type BuildProxyType()
        {
            var type = GetImplementedType(_baseClassType, _interfaceTypes, _interceptorType);
            if (type != null)
                return type;

            type = BuildType();

            AddImplementation(_baseClassType, _interfaceTypes, type, _interceptorType);
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

        private void DefineInterceptorField()
        {
            // private IMethodCallInterceptor _interceptor;
            _interceptorFieldBuilder = _typeBuilder.DefineField("_interceptor", _interceptorType,
                FieldAttributes.Private);
        }

        private void BuildConstructor()
        {
            var constructorBuilder = DeclareConstructor(); // public ProxyClass(IMethodCallInterceptor interceptor)
            ImplementConstructor(constructorBuilder); // : base() { this._interceptor = interceptor; }
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

        private ConstructorBuilder DeclareConstructor()
        {
            var ctorParamTypes = new List<Type> { _interceptorType };

            var baseCtorParams = _baseClassType.GetConstructors()[0].GetParameters();
            ctorParamTypes.AddRange(baseCtorParams.Select(p => p.ParameterType));

            var constructorBuilder = _typeBuilder.DefineConstructor(
                MethodAttributes.Public,
                CallingConventions.HasThis,
                ctorParamTypes.ToArray());

            return constructorBuilder;
        }

        private void ImplementConstructor(ConstructorBuilder constructorBuilder)
        {
            //var baseCtor = _baseClassType.GetConstructor(Type.EmptyTypes);

            //if (baseCtor == null)
            //{
            //    throw new InvalidOperationException("Base class must have parameterless constructor!");
            //}

            var baseCtor = _baseClassType.GetConstructors()[0];
            var baseParamCount = baseCtor.GetParameters().Length;


            var il = constructorBuilder.GetILGenerator();

            // call base ctor
            il.Emit(OpCodes.Ldarg_0); // push this

            for (var i = 0; i < baseParamCount; i++)
            {
                il.Emit(OpCodes.Ldarg, i + 2);
            }

            il.Emit(OpCodes.Call, baseCtor); // Call base constructor this.base(); pops this

            // set _interceptor
            il.Emit(OpCodes.Ldarg_0); // push this
            il.Emit(OpCodes.Ldarg_1); // push interceptor argument
            il.Emit(OpCodes.Stfld, _interceptorFieldBuilder);
            // this._interceptor = interceptor, pop this, pop callhandler argument

            il.Emit(OpCodes.Ret); // exit ctor
        }

        private void BuildMethod(MethodInfo mi)
        {
            var methodBuilder = MethodBuilder.GetInstance(_typeBuilder, mi,
                _interceptorFieldBuilder);
            methodBuilder.Build();
        }
    }
}