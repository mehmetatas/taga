using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Taga.Core.DynamicProxy;
using Taga.Core.Extensions;

namespace Taga.Impl.DynamicProxy.Builder
{
    abstract class CallHandlerMethodBuilder
    {
        private ParameterInfo[] _parameters;
        private MethodBuilder _methodBuilder;
        private readonly TypeBuilder _typeBuilder;
        private readonly FieldBuilder _callHandlerFieldBuilder;

        protected readonly MethodInfo MethodInfo;

        protected ILGenerator IL { get; private set; }
        protected int ParameterCount { get; private set; }

        private MethodInfo _beforeCall;
        private MethodInfo BeforeCall
        {
            get
            {
                return _beforeCall ?? (_beforeCall = typeof(ICallHandler).GetMethods().First(m => m.Name == "BeforeMethodCall"));
            }
        }

        private MethodInfo _afterCall;
        private MethodInfo AfterCall
        {
            get
            {
                return _afterCall ?? (_afterCall = typeof(ICallHandler).GetMethods().First(m => m.Name == "AfterMethodCall"));
            }
        }

        private MethodInfo _onError;
        private MethodInfo OnError
        {
            get
            {
                return _onError ?? (_onError = typeof(ICallHandler).GetMethods().First(m => m.Name == "OnError"));
            }
        }

        protected CallHandlerMethodBuilder(TypeBuilder typeBuilder, MethodInfo methodInfo, FieldBuilder callHandlerFieldBuilder)
        {
            _typeBuilder = typeBuilder;
            MethodInfo = methodInfo;
            _callHandlerFieldBuilder = callHandlerFieldBuilder;
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

        private LocalBuilder _objParameter;
        private void SetObjectParameter()
        {
            // CallHandlera verilecek object obj
            _objParameter = IL.DeclareLocal(typeof(object)); // object obj;
            IL.Emit(OpCodes.Ldarg_0); // push this
            IL.Emit(OpCodes.Stloc, _objParameter); // obj = this; pops this
        }

        private LocalBuilder _methodInfoParameter;
        private void SetMethodInfoParameter()
        {
            // CallHandlera verilecek MethodInfo methodInfo
            _methodInfoParameter = IL.DeclareLocal(typeof(MethodInfo)); // MethodInfo methodInfo;
            IL.Emit(OpCodes.Ldtoken, MethodInfo);
            IL.Emit(OpCodes.Call, typeof(MethodBase).GetMethod(
                "GetMethodFromHandle", new[] { typeof(RuntimeMethodHandle) })); // MethodBase.GetMethodFromHandle(new RuntimeMethodHandle());
            IL.Emit(OpCodes.Stloc, _methodInfoParameter);
        }

        private LocalBuilder _argsParameter;
        private void SetArgsParameters()
        {
            // CallHandlera verilecek object[] args
            _argsParameter = IL.DeclareLocal(typeof(object[])); // object[] args;
            IL.Emit(OpCodes.Ldc_I4, ParameterCount); // push parameterCount as Int32
            IL.Emit(OpCodes.Newarr, typeof(object)); // push new object[parameterCount]; pops parameterCount
            IL.Emit(OpCodes.Stloc, _argsParameter); // args = new object[ParameterCount]; pops new object[parameterCount]

            // Metoda gelen parametreleri args'a doldur
            for (var i = 0; i < ParameterCount; i++)
            {
                var parameterInfo = _parameters[i];

                IL.Emit(OpCodes.Ldloc, _argsParameter); // push args
                IL.Emit(OpCodes.Ldc_I4, i); // push i
                IL.Emit(OpCodes.Ldarg_S, i + 1); // push params[i]; pops i; metoda gelen parametrelerin i'incisi. 0'ıncı parametre this olduğu için  "+1" var
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
            // this._callHandler.BeforeCall(obj, methodInfo, args);
            IL.Emit(OpCodes.Ldarg_0); // push this
            IL.Emit(OpCodes.Ldfld, _callHandlerFieldBuilder); // push _callHandler; pops this
            IL.Emit(OpCodes.Ldloc, _objParameter); // push obj
            IL.Emit(OpCodes.Ldloc, _methodInfoParameter); // push methodInfo 
            IL.Emit(OpCodes.Ldloc, _argsParameter); // push args
            IL.Emit(OpCodes.Call, BeforeCall); // _callHandler.BeforeCall(obj, methodInfo, args); push return value
        }

        protected LocalBuilder ReturnValue;
        protected abstract void SetReturnValue();

        private void InvokeAfterMethodCall()
        {
            // this._callHandler.AfterCall(obj, methodInfo, args, returnValue);
            IL.Emit(OpCodes.Ldarg_0); // push this
            IL.Emit(OpCodes.Ldfld, _callHandlerFieldBuilder); // push _callHandler; pops this
            IL.Emit(OpCodes.Ldloc, _objParameter); // push obj
            IL.Emit(OpCodes.Ldloc, _methodInfoParameter); // push methodInfo 
            IL.Emit(OpCodes.Ldloc, _argsParameter); // push args
            IL.Emit(OpCodes.Ldloc, ReturnValue); // push res
            IL.Emit(OpCodes.Call, AfterCall); // _callHandler.AfterCall(obj, methodInfo, args, returnValue); push return value (void değilse)
        }

        private void Catch()
        {
            var ex = IL.DeclareLocal(typeof(Exception));

            IL.BeginCatchBlock(typeof(Exception));          // catch 
            IL.Emit(OpCodes.Stloc_S, ex);                   // (Exception ex) {
            InvokeOnError(ex);                              //     _callHandler.AfterCall(obj, methodInfo, args);
            IL.EndExceptionBlock();                         // }
        }

        private void InvokeOnError(LocalBuilder exception)
        {
            // this._callHandler.OnError(obj, methodInfo, args);
            IL.Emit(OpCodes.Ldarg_0); // push this
            IL.Emit(OpCodes.Ldfld, _callHandlerFieldBuilder); // push _callHandler; pops this
            IL.Emit(OpCodes.Ldloc, _objParameter); // push obj
            IL.Emit(OpCodes.Ldloc, _methodInfoParameter); // push methodInfo 
            IL.Emit(OpCodes.Ldloc, _argsParameter); // push args
            IL.Emit(OpCodes.Ldloc, exception); // push ex
            IL.Emit(OpCodes.Call, OnError); // _callHandler.AfterCall(obj, methodInfo, args);
        }

        private void Return()
        {
            if (MethodInfo.ReturnType != typeof(void))
            {
                IL.Emit(OpCodes.Ldloc, ReturnValue); // push returnValue
                IL.Emit(OpCodes.Unbox_Any, MethodInfo.ReturnType); // (ReturnType)returnValue
            }

            IL.Emit(OpCodes.Ret); // returns the value on the stack, if ReturnType is void stack should be empty
        }

        internal void Build()
        {
            Declare();                   // public override? ReturnType Method(arguments...) {
            SetObjectParameter();        //     object obj = this;
            SetMethodInfoParameter();    //     MethodInfo methodInfo = MethodBase.GetMethodFromHandle(new RuntimeMethodHandle());
            SetArgsParameters();         //     object[] args = arguments;
            Try();                       //     try {
            InvokeBeforeMethodCall();    //         object returnValue = _callHandler.BeforeMethodCall(obj, methodInfo, args);
            SetReturnValue();            //         !IsAbstract => returnValue = (object)base.Method(arguments);
            InvokeAfterMethodCall();     //         _callHandler.AfterMethodCall(obj, methodInfo, args, returnValue);
            Catch();                     //      } catch (Exception ex) { _callHandler.OnError(obj, methodInfo, args, ex); }
            Return();                    //     IsVoid ? (return;) : return (ReturnType)returnValue; }
        }

        internal static CallHandlerMethodBuilder GetInstance(TypeBuilder typeBuilder, MethodInfo methodInfo, FieldBuilder callHandlerFieldBuilder)
        {
            if (methodInfo.IsAbstract)
                return new CallHandlerMethodImplementor(typeBuilder, methodInfo, callHandlerFieldBuilder);
            return new CallHandlerMethodOverrider(typeBuilder, methodInfo, callHandlerFieldBuilder);
        }
    }
}
