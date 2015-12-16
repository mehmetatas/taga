using Taga.Orm.Sql.Where.Expressions;

namespace Taga.Orm.Sql.Where
{
    public interface IWhereExpressionVisitor
    {
        void Visit(LogicalExpression e);
        void Visit(ColumnExpression e);
        void Visit(ValueExpression e);
        void Visit(NullExpression e);
        void Visit(BinaryExpression e);
        void Visit(NotExpression e);
        void Visit(LikeExpression e);
        void Visit(InExpression e);
    }
}