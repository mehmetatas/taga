namespace Taga.Orm.Sql.Where.Expressions
{
    public class NotExpression : IWhereExpression
    {
        public IWhereExpression Operand { get; set; }

        public void Accept(IWhereExpressionVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}