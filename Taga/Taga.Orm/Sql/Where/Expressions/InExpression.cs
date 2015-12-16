namespace Taga.Orm.Sql.Where.Expressions
{
    public class InExpression : IWhereExpression
    {
        public ColumnExpression Column { get; set; }
        public ValueExpression Values { get; set; }

        public void Accept(IWhereExpressionVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}