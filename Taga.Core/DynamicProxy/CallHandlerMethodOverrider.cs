using System.Reflection;
using System.Reflection.Emit;

namespace Taga.Core.DynamicProxy
{
    internal class CallHandlerMethodOverrider : CallHandlerMethodBuilder
    {
        internal CallHandlerMethodOverrider(TypeBuilder typeBuilder, MethodInfo methodInfo,
            FieldBuilder callHandlerFieldBuilder)
            : base(typeBuilder, methodInfo, callHandlerFieldBuilder)
        {
        }

        protected override void SetReturnValue()
        {
            // ReturnValue = base.Method(args...)
            CallBaseMethod();
            // stack'ta base'den dönen değer var
            SetReturnValueFromBase();
        }

        private void CallBaseMethod()
        {
            IL.Emit(OpCodes.Pop); // pop return value of BeforeCall

            // base'den Method'u çağır
            // returnValue = base.Method(params...)
            IL.Emit(OpCodes.Ldarg_0); // push this 
            for (var i = 0; i < ParameterCount; i++) // metoda gelen parametreleri stack'e at
                IL.Emit(OpCodes.Ldarg_S, i + 1); // push params[i]
            IL.Emit(OpCodes.Call, MethodInfo); // base.Method(params) pop this, pop params push return value
        }

        private void SetReturnValueFromBase()
        {
            ReturnValue = IL.DeclareLocal(typeof (object));

            if (MethodInfo.ReturnType == typeof (void))
                return;

            // unbox returnValue if required
            if (MethodInfo.ReturnType.IsValueType)
                IL.Emit(OpCodes.Box, MethodInfo.ReturnType);
            IL.Emit(OpCodes.Stloc, ReturnValue); // pop return value into res
        }
    }
}