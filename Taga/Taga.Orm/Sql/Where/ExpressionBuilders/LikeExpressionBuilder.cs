using Taga.Orm.Sql.Where.Expressions;
using Taga.Orm.Sql.Where.ExpressionVisitors;

namespace Taga.Orm.Sql.Where.ExpressionBuilders
{
    public class LikeExpressionBuilder : WhereExpressionBuilder
    {
        private readonly LikeExpression _expression;

        public LikeExpressionBuilder(Operator oper)
        {
            _expression = new LikeExpression
            {
                Operator = oper
            };
        }

        public override void Visit(ValueExpression e)
        {
            _expression.Value = e;
        }

        public override void Visit(ColumnExpression e)
        {
            _expression.Column = e;
        }

        public override IWhereExpression Build()
        {
            FixColumnMeta(_expression.Value, _expression.Column);

            return _expression;
        }
    }
}