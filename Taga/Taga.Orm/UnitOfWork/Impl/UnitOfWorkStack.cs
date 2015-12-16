using System.Collections.Generic;

namespace Taga.Orm.UnitOfWork.Impl
{
    public class UnitOfWorkStack : IUnitOfWorkStack
    {
        private readonly Stack<IUnitOfWork> _stack;

        public UnitOfWorkStack()
        {
            _stack = new Stack<IUnitOfWork>();
        }

        public void Push(IUnitOfWork unitOfWork)
        {
            _stack.Push(unitOfWork);
        }

        public IUnitOfWork Pop()
        {
            return _stack.Pop();
        }

        public IUnitOfWork Current => _stack.Peek();
    }
}