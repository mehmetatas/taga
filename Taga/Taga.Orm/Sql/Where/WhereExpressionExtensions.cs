using System;
using Taga.Orm.Sql.Where.Expressions;

namespace Taga.Orm.Sql.Where
{
    public static class WhereExpressionExtensions
    {
        public static void SetOperand(this BinaryExpression expression, IWhereExpression operand)
        {
            if (expression.Operand1 == null)
            {
                expression.Operand1 = operand;
            }
            else if (expression.Operand2 == null)
            {
                expression.Operand2 = operand;
            }
            else
            {
                throw new InvalidOperationException("All operands are alreadey set!");
            }
        }

        public static void SetOperand(this LogicalExpression expression, IWhereExpression operand)
        {
            if (expression.Operand1 == null)
            {
                expression.Operand1 = operand;
            }
            else if (expression.Operand2 == null)
            {
                expression.Operand2 = operand;
            }
            else
            {
                throw new InvalidOperationException("All operands are alreadey set!");
            }
        }
    }
}