namespace Taga.Orm.Sql.Where
{
    public interface IWhereExpression
    {
        void Accept(IWhereExpressionVisitor visitor);
    }
}