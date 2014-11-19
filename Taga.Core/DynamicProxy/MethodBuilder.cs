using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using EmitMethodBuilder = System.Reflection.Emit.MethodBuilder;

namespace Taga.Core.DynamicProxy
{
    internal abstract class MethodBuilder
    {
        protected readonly MethodInfo MethodInfo;
        private readonly FieldBuilder _interceptorFieldBuilder;
        private readonly TypeBuilder _typeBuilder;
        protected LocalBuilder ReturnValue;
        private MethodInfo _afterCall;
        private LocalBuilder _argsParameter;
        private MethodInfo _beforeCall;
        private EmitMethodBuilder _methodBuilder;
        private LocalBuilder _methodInfoParameter;
        private LocalBuilder _objParameter;
        private MethodInfo _onError;
        private ParameterInfo[] _parameters;

        protected MethodBuilder(TypeBuilder typeBuilder, MethodInfo methodInfo,
            FieldBuilder interceptorFieldBuilder)
        {
            _typeBuilder = typeBuilder;
            MethodInfo = methodInfo;
            _interceptorFieldBuilder = interceptorFieldBuilder;
        }

        protected ILGenerator IL { get; private set; }
        protected int ParameterCount { get; private set; }

        private MethodInfo BeforeCall
        {
            get
            {
                return _beforeCall ??
                       (_beforeCall = typeof (IMethodCallInterceptor).GetMethods().First(m => m.Name == "BeforeCall"));
            }
        }

        private MethodInfo AfterCall
        {
            get
            {
                return _afterCall ?? (_afterCall = typeof (IMethodCallInterceptor).GetMethods().First(m => m.Name == "AfterCall"));
            }
        }

        private MethodInfo OnError
        {
            get { return _onError ?? (_onError = typeof (IMethodCallInterceptor).GetMethods().First(m => m.Name == "OnError")); }
        }

        private void Declare()
        {
            // public override? ReturnType Method(arguments...)
            _methodBuilder = _typeBuilder.DefineMethod(MethodInfo.Name,
                MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual,
                MethodInfo.ReturnType,
                MethodInfo.GetParameterTypes());
            IL = _methodBuilder.GetILGenerator();

            _parameters = MethodInfo.GetParameters();
            ParameterCount = _parameters.Length;
        }

        private void SetObjectParameter()
        {
            // Interceptora verilecek object obj
            _objParameter = IL.DeclareLocal(typeof (object)); // object obj;
            IL.Emit(OpCodes.Ldarg_0); // push this
            IL.Emit(OpCodes.Stloc, _objParameter); // obj = this; pops this
        }

        private void SetMethodInfoParameter()
        {
            // Interceptora verilecek MethodInfo methodInfo
            _methodInfoParameter = IL.DeclareLocal(typeof (MethodInfo)); // MethodInfo methodInfo;
            IL.Emit(OpCodes.Ldtoken, MethodInfo);
            IL.Emit(OpCodes.Call, typeof (MethodBase).GetMethod(
                "GetMethodFromHandle", new[] {typeof (RuntimeMethodHandle)}));
            // MethodBase.GetMethodFromHandle(new RuntimeMethodHandle());
            IL.Emit(OpCodes.Stloc, _methodInfoParameter);
        }

        private void SetArgsParameters()
        {
            // Interceptora verilecek object[] args
            _argsParameter = IL.DeclareLocal(typeof (object[])); // object[] args;
            IL.Emit(OpCodes.Ldc_I4, ParameterCount); // push parameterCount as Int32
            IL.Emit(OpCodes.Newarr, typeof (object)); // push new object[parameterCount]; pops parameterCount
            IL.Emit(OpCodes.Stloc, _argsParameter);
            // args = new object[ParameterCount]; pops new object[parameterCount]

            // Metoda gelen parametreleri args'a doldur
            for (var i = 0; i < ParameterCount; i++)
            {
                var parameterInfo = _parameters[i];

                IL.Emit(OpCodes.Ldloc, _argsParameter); // push args
                IL.Emit(OpCodes.Ldc_I4, i); // push i
                IL.Emit(OpCodes.Ldarg_S, i + 1);
                // push params[i]; pops i; metoda gelen parametrelerin i'incisi. 0'ıncı parametre this olduğu için  "+1" var
                if (parameterInfo.ParameterType.IsPrimitive || parameterInfo.ParameterType.IsValueType)
                    IL.Emit(OpCodes.Box, parameterInfo.ParameterType); // (object)params[i]
                IL.Emit(OpCodes.Stelem_Ref); // args[i] = (object)params[i]; pops params[i]
            }
        }

        private void Try()
        {
            IL.BeginExceptionBlock(); // try {
        }

        private void InvokeBeforeMethodCall()
        {
            // this._interceptor.BeforeCall(obj, methodInfo, args);
            IL.Emit(OpCodes.Ldarg_0); // push this
            IL.Emit(OpCodes.Ldfld, _interceptorFieldBuilder); // push _interceptor; pops this
            IL.Emit(OpCodes.Ldloc, _objParameter); // push obj
            IL.Emit(OpCodes.Ldloc, _methodInfoParameter); // push methodInfo 
            IL.Emit(OpCodes.Ldloc, _argsParameter); // push args
            IL.Emit(OpCodes.Call, BeforeCall); // _interceptor.BeforeCall(obj, methodInfo, args); push return value
        }

        protected abstract void SetReturnValue();

        private void InvokeAfterMethodCall()
        {
            // this._interceptor.AfterCall(obj, methodInfo, args, returnValue);
            IL.Emit(OpCodes.Ldarg_0); // push this
            IL.Emit(OpCodes.Ldfld, _interceptorFieldBuilder); // push _interceptor; pops this
            IL.Emit(OpCodes.Ldloc, _objParameter); // push obj
            IL.Emit(OpCodes.Ldloc, _methodInfoParameter); // push methodInfo 
            IL.Emit(OpCodes.Ldloc, _argsParameter); // push args
            IL.Emit(OpCodes.Ldloc, ReturnValue); // push res
            IL.Emit(OpCodes.Call, AfterCall);
            // _interceptor.AfterCall(obj, methodInfo, args, returnValue); push return value (void değilse)
        }

        private void Catch()
        {
            var ex = IL.DeclareLocal(typeof (Exception));

            IL.BeginCatchBlock(typeof (Exception)); // catch 
            IL.Emit(OpCodes.Stloc_S, ex); // (Exception ex) {
            InvokeOnError(ex); //     _interceptor.OnError(obj, methodInfo, args, ex);
            
            IL.Emit(OpCodes.Ldloc, ex); //     push ex
            IL.Emit(OpCodes.Throw); //     throw;

            IL.EndExceptionBlock(); // }
        }

        private void InvokeOnError(LocalBuilder exception)
        {
            // this._interceptor.OnError(obj, methodInfo, args, ec);
            IL.Emit(OpCodes.Ldarg_0); // push this
            IL.Emit(OpCodes.Ldfld, _interceptorFieldBuilder); // push _interceptor; pops this
            IL.Emit(OpCodes.Ldloc, _objParameter); // push obj
            IL.Emit(OpCodes.Ldloc, _methodInfoParameter); // push methodInfo 
            IL.Emit(OpCodes.Ldloc, _argsParameter); // push args
            IL.Emit(OpCodes.Ldloc, exception); // push ex
            IL.Emit(OpCodes.Call, OnError); // _interceptor.OnError(obj, methodInfo, args, ex);
        }

        private void Return()
        {
            if (MethodInfo.ReturnType != typeof (void))
            {
                IL.Emit(OpCodes.Ldloc, ReturnValue); // push returnValue
                IL.Emit(OpCodes.Unbox_Any, MethodInfo.ReturnType); // (ReturnType)returnValue
            }

            IL.Emit(OpCodes.Ret); // returns the value on the stack, if ReturnType is void stack should be empty
        }

        internal void Build()
        {
            Declare(); // public override? ReturnType Method(arguments...) {
            SetObjectParameter(); //     object obj = this;
            SetMethodInfoParameter();
            //     MethodInfo methodInfo = MethodBase.GetMethodFromHandle(new RuntimeMethodHandle());
            SetArgsParameters(); //     object[] args = arguments;
            Try(); //     try {
            InvokeBeforeMethodCall();
            //         object returnValue = _interceptor.BeforeMethodCall(obj, methodInfo, args);
            SetReturnValue(); //         !IsAbstract => returnValue = (object)base.Method(arguments);
            InvokeAfterMethodCall(); //         _interceptor.AfterMethodCall(obj, methodInfo, args, returnValue);
            Catch(); //      } catch (Exception ex) { _interceptor.OnError(obj, methodInfo, args, ex); }
            Return(); //     IsVoid ? (return;) : return (ReturnType)returnValue; }
        }

        internal static MethodBuilder GetInstance(TypeBuilder typeBuilder, MethodInfo methodInfo,
            FieldBuilder interceptorFieldBuilder)
        {
            if (methodInfo.IsAbstract)
                return new MethodImplementor(typeBuilder, methodInfo, interceptorFieldBuilder);
            return new MethodOverrider(typeBuilder, methodInfo, interceptorFieldBuilder);
        }
    }
}