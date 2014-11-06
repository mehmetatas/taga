using System;
using System.Collections.Generic;
using System.Reflection;

namespace Taga.SimpLinq.QueryBuilder
{
    public interface ISelectQuery
    {
        Type FromType { get; }
        IReadOnlyCollection<PropertyInfo> SelectProperties { get; }
        IReadOnlyCollection<IJoin> LeftJoinProperties { get; }
        IReadOnlyCollection<IJoin> InnerJoinProperties { get; }
        IReadOnlyCollection<IOrderBy> OrderByProperties { get; }
        IWhere Where { get; }
        IPage Page { get; }

        string ToSql();
    }
}
