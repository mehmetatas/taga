using Taga.Orm.Sql.Where.Expressions;
using Taga.Orm.Sql.Where.ExpressionVisitors;

namespace Taga.Orm.Sql.Where.ExpressionBuilders
{
    public class LogicalExpressionBuilder : WhereExpressionBuilder
    {
        private readonly LogicalExpression _expression;

        public LogicalExpressionBuilder(Operator oper)
        {
            _expression = new LogicalExpression
            {
                Operator = oper
            };
        }
        
        public override void Visit(BinaryExpression e)
        {
            _expression.SetOperand(e);
        }

        public override void Visit(InExpression e)
        {
            _expression.SetOperand(e);
        }

        public override void Visit(LikeExpression e)
        {
            _expression.SetOperand(e);
        }

        public override void Visit(LogicalExpression e)
        {
            _expression.SetOperand(e);
        }

        public override void Visit(NotExpression e)
        {
            _expression.SetOperand(e);
        }

        public override IWhereExpression Build()
        {
            return _expression;
        }
    }
}