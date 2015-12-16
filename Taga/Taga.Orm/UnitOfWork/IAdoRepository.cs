using System.Collections.Generic;
using Taga.Orm.Sql.Command;

namespace Taga.Orm.UnitOfWork
{
    public interface IAdoRepository
    {
        IList<T> ExecuteQuery<T>(Command cmd) where T : class, new();

        int ExecuteNonQuery(Command cmd);

        object ExecuteScalar(Command cmd);
    }
}