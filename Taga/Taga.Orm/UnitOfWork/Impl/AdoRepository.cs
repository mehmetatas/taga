using System.Collections.Generic;
using Taga.Orm.Db;
using Taga.Orm.Sql.Command;

namespace Taga.Orm.UnitOfWork.Impl
{
    public class AdoRepository : IAdoRepository
    {
        private static IDb Db => UnitOfWork.Current.Db;

        private void EnsureTransaction()
        {
            UnitOfWork.Current.EnsureTransaction();
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