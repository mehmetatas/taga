using System.Collections.Generic;
using Taga.Core.Context;

namespace Taga.Core.Repository.Base
{
    public abstract class UnitOfWork : IUnitOfWork
    {
        protected UnitOfWork()
        {
            Push(this);
        }

        public abstract void Save();

        public void Dispose()
        {
            Pop();
            OnDispose();
        }

        protected virtual void OnDispose()
        {

        }

        #region Stack

        private static Stack<IUnitOfWork> Stack
        {
            get
            {
                var uowStack = CallContext.Get<Stack<IUnitOfWork>>("UnitOfWorkStack");
                if (uowStack == null)
                {
                    uowStack = new Stack<IUnitOfWork>();
                    CallContext.Set("UnitOfWorkStack", uowStack);
                }
                return uowStack;
            }
        }

        internal static IUnitOfWork Current
        {
            get { return Stack.Peek(); }
        }

        private static void Push(IUnitOfWork uow)
        {
            Stack.Push(uow);
        }

        private static void Pop()
        {
            Stack.Pop();
        }

        #endregion
    }
}
