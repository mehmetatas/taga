using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Taga.Core.Repository.Hybrid
{
    public interface IHybridAdapter
    {
        ITransactionalUnitOfWork UnitOfWork { get; }

        IDbCommand CreateCommand();

        IQueryable<T> Select<T>() where T : class, new();

        IList<T> Query<T>(string spNameOrSql, IDictionary<string, object> args = null, bool rawSql = false)
            where T : class, new();
    }
}
