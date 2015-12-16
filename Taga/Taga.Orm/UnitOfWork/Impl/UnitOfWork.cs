using System;
using System.Data;
using Taga.Orm.Db;

namespace Taga.Orm.UnitOfWork.Impl
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IDbFactory _factory;
        private readonly IUnitOfWorkStack _stack;

        private bool _isTransactional;
        private IsolationLevel _isolationLevel;

        private IDb _db;
        internal IDb Db => _db ?? (_db = _factory.Create());

        public UnitOfWork(IDbFactory factory, IUnitOfWorkStack stack)
        {
            _factory = factory;
            _stack = stack;
            _stack.Push(this);
        }

        internal void EnsureTransaction()
        {
            if (_isTransactional)
            {
                Db.BeginTransaction(_isolationLevel);
            }
        }

        public void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            _isTransactional = true;
            _isolationLevel = isolationLevel;
        }

        public void Rollback()
        {
            _db?.Rollback();
        }

        public void Commit()
        {
            _db?.Commit();
        }

        public void Dispose()
        {
            var uow = _stack.Pop();

            _db?.Dispose();

            if (uow != this)
            {
                throw new InvalidOperationException("Unexpected UnitOfWork found in stack!");
            }
        }

        #region Stack

        //private static Stack<UnitOfWork> Stack
        //{
        //    get
        //    {
        //        // TODO: make sure consistency and thread safety of CallContext
        //        var uowStack = (Stack<UnitOfWork>)CallContext.GetData("UnitOfWorkStack");
        //        if (uowStack == null)
        //        {
        //            uowStack = new Stack<UnitOfWork>();
        //            CallContext.SetData("UnitOfWorkStack", uowStack);
        //        }
        //        return uowStack;
        //    }
        //}

        //internal static UnitOfWork Current
        //{
        //    get
        //    {
        //        if (Stack.Count == 0)
        //        {
        //            throw new InvalidOperationException("No UnitOfWork is available!");
        //        }
        //        return Stack.Peek();
        //    }
        //}

        //private static void Push(UnitOfWork uow)
        //{
        //    Stack.Push(uow);
        //}

        //private static void Pop()
        //{
        //    Stack.Pop();
        //}

        #endregion
    }
}
