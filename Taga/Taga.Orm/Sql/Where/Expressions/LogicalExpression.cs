namespace Taga.Orm.Sql.Where.Expressions
{
    /// <summary>
    /// and, or
    /// </summary>
    public class LogicalExpression : IWhereExpression
    {
        public IWhereExpression Operand1 { get; set; }
        public Operator Operator { get; set; }
        public IWhereExpression Operand2 { get; set; }

        public void Accept(IWhereExpressionVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}