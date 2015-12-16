using System;
using System.Linq.Expressions;
using Taga.Orm.Meta;
using Taga.Orm.Sql.Where.ExpressionVisitors;

namespace Taga.Orm.Sql.Where
{
    public interface IWhereCommandBuilder : IWhereExpressionVisitor
    {
        Command.Command Build();
    }

    public static class WhereCommandBuilderExtensions
    {
        public static Command.Command Build<T>(this IWhereCommandBuilder whereCmdBuilder, IDbMeta meta, Expression<Func<T, bool>> filter, IWhereExpressionListener listener)
        {
            var whereExp = WhereExpressionVisitor.Build(meta, filter, listener);
            whereExp.Accept(whereCmdBuilder);
            return whereCmdBuilder.Build();
        }
    }
}