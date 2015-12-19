using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;

namespace Taga.Orm.UnitOfWork.Impl
{
    public class UnitOfWorkStack : IUnitOfWorkStack
    {
        private const string StackKey = "Taga.Orm.UnitOfWork";

        private static Stack<IUnitOfWork> Stack
        {
            get
            {
                var stack = CallContext.LogicalGetData(StackKey) as Stack<IUnitOfWork>;

                if (stack == null)
                {
                    stack = new Stack<IUnitOfWork>();
                    CallContext.LogicalSetData(StackKey, stack);
                }

                return stack;
            }
        }

        public void Push(IUnitOfWork unitOfWork)
        {
            Stack.Push(unitOfWork);
        }

        public IUnitOfWork Pop()
        {
            return Stack.Pop();
        }

        public IUnitOfWork Current => Stack.Peek();
    }
}