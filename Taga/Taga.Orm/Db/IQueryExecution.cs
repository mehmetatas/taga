using System.Collections.Generic;
using Taga.Orm.Sql;

namespace Taga.Orm.Db
{
    public interface IQueryExecution<T> where T : class, new()
    {
        T FirstOrDefault();

        List<T> ToList();

        Page<T> Page(int page, int pageSize);

        Page<T> Top(int top);
    }
}