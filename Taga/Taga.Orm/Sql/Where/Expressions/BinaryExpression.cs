namespace Taga.Orm.Sql.Where.Expressions
{
    /// <summary>
    /// eq, noteq, lt, gt, lte, gte
    /// </summary>
    public class BinaryExpression : IWhereExpression
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