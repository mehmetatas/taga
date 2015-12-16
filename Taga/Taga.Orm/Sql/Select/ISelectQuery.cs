using System.Collections.Generic;
using Taga.Orm.Sql.Where;

namespace Taga.Orm.Sql.Select
{
    public interface ISelectQuery
    {
        Table From { get; }
        IDictionary<string, Column> SelectColumns { get; }
        IDictionary<string, Join> Joins { get; }
        List<IWhereExpression> WhereExpressions { get; }
        List<OrderBy> OrderByColumns { get; }

        int Page { get; }
        int PageSize { get; }
    }

    public static class SelectQueryExtensions
    {
        public static bool IsPaging(this ISelectQuery query)
        {
            return query.Page > 0 && query.PageSize > 0;
        }
        public static bool IsTop(this ISelectQuery query)
        {
            return query.Page < 1 && query.PageSize > 0;
        }
    }
}