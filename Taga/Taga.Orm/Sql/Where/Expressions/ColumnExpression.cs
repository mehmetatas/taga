
namespace Taga.Orm.Sql.Where.Expressions
{
    public class ColumnExpression : IWhereExpression
    {
        public Column Column { get; set; }

        public void Accept(IWhereExpressionVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}