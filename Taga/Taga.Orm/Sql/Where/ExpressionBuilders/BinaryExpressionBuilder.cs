using Taga.Orm.Sql.Where.Expressions;
using Taga.Orm.Sql.Where.ExpressionVisitors;

namespace Taga.Orm.Sql.Where.ExpressionBuilders
{
    public class BinaryExpressionBuilder : WhereExpressionBuilder
    {
        private readonly BinaryExpression _expression;

        public BinaryExpressionBuilder(Operator oper)
        {
            _expression = new BinaryExpression
            {
                Operator = oper
            };
        }

        public override void Visit(ColumnExpression e)
        {
            _expression.SetOperand(e);
        }

        public override void Visit(ValueExpression e)
        {
            _expression.SetOperand(e);
        }

        public override void Visit(NullExpression e)
        {
            _expression.SetOperand(e);
        }

        public override IWhereExpression Build()
        {
            FixColumnMeta(_expression.Operand1 as ValueExpression, _expression.Operand2 as ColumnExpression);
            FixColumnMeta(_expression.Operand2 as ValueExpression, _expression.Operand1 as ColumnExpression);

            return _expression;
        }
    }
}