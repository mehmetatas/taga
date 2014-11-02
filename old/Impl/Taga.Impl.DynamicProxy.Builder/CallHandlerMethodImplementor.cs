using System.Reflection;
using System.Reflection.Emit;

namespace Taga.Impl.DynamicProxy.Builder
{
    internal class CallHandlerMethodImplementor : CallHandlerMethodBuilder
    {
        internal CallHandlerMethodImplementor(TypeBuilder typeBuilder, MethodInfo methodInfo,
                                              FieldBuilder callHandlerFieldBuilder)
            : base(typeBuilder, methodInfo, callHandlerFieldBuilder)
        {
        }

        protected override void SetReturnValue()
        {
            // object res = returnValue;
            ReturnValue = IL.DeclareLocal(typeof (object));
            if (MethodInfo.ReturnType != typeof (void))
                IL.Emit(OpCodes.Stloc, ReturnValue); // pop return value of BeforeCall into res
            else
                IL.Emit(OpCodes.Pop); // pop return value of BeforeCall
        }
    }
}