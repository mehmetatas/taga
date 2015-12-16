using System.Collections.Generic;
using Taga.Orm.Meta;

namespace Taga.Orm.Sql.Where
{
    public interface IWhereExpressionListener
    {
        Column RegisterColumn(IList<ColumnMeta> propChain);
    }
}