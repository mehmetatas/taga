using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Taga.SimpLinq.QueryBuilder.Impl
{
    public class WhereExpressionBuilder : ExpressionVisitor
    {
        private readonly Stack<Where> _stack = new Stack<Where>();

        private Where _current;

        private WhereExpressionBuilder()
        {

        }

        public static IWhere Build<T>(Expression<Func<T, bool>> whereExpression)
        {
            var evaled = Evaluator.PartialEval(whereExpression);
            var builder = new WhereExpressionBuilder();
            builder.Visit(evaled);
            return builder._current;
        }

        private static Expression StripQuotes(Expression expression)
        {
            while (expression.NodeType == ExpressionType.Quote)
            {
                expression = ((UnaryExpression)expression).Operand;
            }
            return expression;
        }

        private void Push()
        {
            _stack.Push(_current);
            _current = new Where();
        }

        private void Pop()
        {
            var current = _stack.Pop();
            if (current != null)
            {
                current.SetOperand(_current);
                _current = current;
            }
        }

        protected override Expression VisitMethodCall(MethodCallExpression methodCallExpression)
        {
            if (methodCallExpression.Method.DeclaringType == typeof(string) && methodCallExpression.Method.Name == "StartsWith")
            {
                var expression = StripQuotes(methodCallExpression.Arguments[0]);

                Push();
                _current.Operator = Operator.LikeStartsWith;
                Visit(methodCallExpression.Object);
                Visit(expression);
                Pop();

                return methodCallExpression;
            }
            if (methodCallExpression.Method.DeclaringType == typeof(string) && methodCallExpression.Method.Name == "EndsWith")
            {
                var expression = StripQuotes(methodCallExpression.Arguments[0]);

                Push();
                _current.Operator = Operator.LikeEndsWith;
                Visit(methodCallExpression.Object);
                Visit(expression);
                Pop();

                return methodCallExpression;
            }
            if (methodCallExpression.Method.DeclaringType == typeof(string) && methodCallExpression.Method.Name == "Contains")
            {
                var expression = StripQuotes(methodCallExpression.Arguments[0]);

                Push();
                _current.Operator = Operator.LikeContains;
                Visit(methodCallExpression.Object);
                Visit(expression);
                Pop();

                return methodCallExpression;
            }
            if (methodCallExpression.Method.DeclaringType == typeof(Enumerable) && methodCallExpression.Method.Name == "Contains")
            {
                var listExp = StripQuotes(methodCallExpression.Arguments[0]);
                var propExp = StripQuotes(methodCallExpression.Arguments[1]);

                Push();
                _current.Operator = Operator.In;
                Visit(propExp);
                Visit(listExp);
                Pop();

                return methodCallExpression;
            }
            if (methodCallExpression.Method.DeclaringType.IsGenericType && methodCallExpression.Method.DeclaringType.GetGenericTypeDefinition() == typeof(List<>) && methodCallExpression.Method.Name == "Contains")
            {
                var expression = StripQuotes(methodCallExpression.Arguments[0]);

                Push();
                _current.Operator = Operator.In;
                Visit(expression);
                Visit(methodCallExpression.Object);
                Pop();

                return methodCallExpression;
            }

            throw new NotSupportedException(string.Format("The method '{0}' is not supported", methodCallExpression.Method.Name));
        }

        protected override Expression VisitUnary(UnaryExpression unaryExpression)
        {
            switch (unaryExpression.NodeType)
            {
                case ExpressionType.Not:
                    Push();
                    Visit(unaryExpression.Operand);
                    _current = (Where)_current.Operand1;
                    _current.Not();
                    Pop();
                    break;
                case ExpressionType.Convert:
                    Visit(unaryExpression.Operand);
                    break;
                default:
                    throw new NotSupportedException(string.Format("The unary operator '{0}' is not supported",
                        unaryExpression.NodeType));
            }

            return unaryExpression;
        }

        protected override Expression VisitBinary(BinaryExpression binaryExpression)
        {
            Push();

            _current.Operator = GetBinaryOperator(binaryExpression.NodeType);

            Visit(binaryExpression.Left);
            Visit(binaryExpression.Right);

            Pop();

            return binaryExpression;
        }

        protected override Expression VisitConstant(ConstantExpression constantExpression)
        {
            if (constantExpression.Value == null)
            {
                _current.SetOperand(null);
            }
            else
            {
                switch (Type.GetTypeCode(constantExpression.Value.GetType()))
                {
                    case TypeCode.Object:
                        if (constantExpression.Value is IEnumerable && _current.Operator == Operator.In)
                        {
                            _current.SetOperand(constantExpression.Value);
                        }
                        else
                        {
                            throw new NotSupportedException(string.Format("The constant for '{0}' is not supported", constantExpression.Value));
                        }
                        break;
                    default:
                        _current.SetOperand(constantExpression.Value);
                        break;
                }
            }

            return constantExpression;
        }

        protected override Expression VisitMember(MemberExpression memberExpression)
        {
            if (memberExpression.Expression != null && memberExpression.Expression.NodeType == ExpressionType.Parameter)
            {
                _current.SetOperand(memberExpression.Member);
                return memberExpression;
            }

            throw new NotSupportedException(string.Format("The member '{0}' is not supported", memberExpression.Member.Name));
        }

        private static Operator GetBinaryOperator(ExpressionType nodeType)
        {
            switch (nodeType)
            {
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    return Operator.And;
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    return Operator.Or;
                case ExpressionType.Equal:
                    return Operator.Equals;
                case ExpressionType.NotEqual:
                    return Operator.NotEquals;
                case ExpressionType.LessThan:
                    return Operator.LessThan;
                case ExpressionType.LessThanOrEqual:
                    return Operator.LessThanOrEquals;
                case ExpressionType.GreaterThan:
                    return Operator.GreaterThan;
                case ExpressionType.GreaterThanOrEqual:
                    return Operator.GreaterThanOrEquals;
                default:
                    throw new NotSupportedException(string.Format("The binary operator '{0}' is not supported", nodeType));
            }
        }
    }
}