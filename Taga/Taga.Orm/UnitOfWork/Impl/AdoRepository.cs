using System.Collections.Generic;
using Taga.Orm.Db;
using Taga.Orm.Sql.Command;

namespace Taga.Orm.UnitOfWork.Impl
{
    public class AdoRepository : IAdoRepository
    {
        private readonly IUnitOfWorkStack _stack;

        private UnitOfWork UnitOfWork => (UnitOfWork)_stack.Current;

        private IDb Db => UnitOfWork.Db;

        private void EnsureTransaction()
        {
            UnitOfWork.EnsureTransaction();
        }

        public AdoRepository(IUnitOfWorkStack stack)
        {
            _stack = stack;
        }

        public IList<T> ExecuteQuery<T>(Command cmd) where T : class, new()
        {
            return Db.ExecuteQuery<T>(cmd);
        }

        public int ExecuteNonQuery(Command cmd)
        {
            EnsureTransaction();
            return Db.ExecuteNonQuery(cmd);
        }

        public object ExecuteScalar(Command cmd)
        {
            EnsureTransaction();
            return Db.ExecuteScalar(cmd);
        }
    }
}