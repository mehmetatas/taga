using System.Reflection;
using System.Reflection.Emit;

namespace Taga.Core.DynamicProxy
{
    internal class MethodImplementor : MethodBuilder
    {
        internal MethodImplementor(TypeBuilder typeBuilder, MethodInfo methodInfo,
            FieldBuilder callHandlerFieldBuilder)
            : base(typeBuilder, methodInfo, callHandlerFieldBuilder)
        {
        }

        protected override void SetReturnValue()
        {
            // object res = returnValue;
            ReturnValue = IL.DeclareLocal(typeof (object));
            if (MethodInfo.ReturnType == typeof (void))
                IL.Emit(OpCodes.Pop); // pop return value of BeforeCall
            else
                IL.Emit(OpCodes.Stloc, ReturnValue); // pop return value of BeforeCall into res
        }
    }
}