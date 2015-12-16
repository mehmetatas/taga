namespace Taga.Orm.Sql.Where
{
    public interface IWhereExpressionBuilder : IWhereExpressionVisitor
    {
        IWhereExpression Build();
    }
}