using System;
using Taga.Orm.Sql.Where.Expressions;

namespace Taga.Orm.Sql.Where.ExpressionVisitors
{
    /// <summary>
    /// Visits and Builds IWhereExpressions
    /// Acts like State in WhereExpressionVisitor
    /// </summary>
    public abstract class WhereExpressionBuilder : IWhereExpressionBuilder
    {
        public virtual void Visit(LogicalExpression e)
        {
            throw new NotSupportedException();
        }

        public virtual void Visit(ColumnExpression e)
        {
            throw new NotSupportedException();
        }

        public virtual void Visit(ValueExpression e)
        {
            throw new NotSupportedException();
        }

        public virtual void Visit(NullExpression e)
        {
            throw new NotSupportedException();
        }

        public virtual void Visit(BinaryExpression e)
        {
            throw new NotSupportedException();
        }

        public virtual void Visit(NotExpression e)
        {
            throw new NotSupportedException();
        }

        public virtual void Visit(LikeExpression e)
        {
            throw new NotSupportedException();
        }

        public virtual void Visit(InExpression e)
        {
            throw new NotSupportedException();
        }

        public abstract IWhereExpression Build();

        protected static void FixColumnMeta(ValueExpression valEx, ColumnExpression colEx)
        {
            if (valEx != null && colEx != null)
            {
                valEx.ColumnMeta = colEx.Column.Meta;
            }
        }
    }
}