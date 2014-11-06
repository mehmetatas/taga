using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Taga.SimpLinq.QueryBuilder.Impl
{
    public class WhereExpressionBuilder : ExpressionVisitor
    {
        private readonly StringBuilder _sql = new StringBuilder();

        private readonly Stack<Where> _stack = new Stack<Where>();

        private Where _current;

        private WhereExpressionBuilder()
        {

        }

        public static IWhere Build<T>(Expression<Func<T, bool>> expression)
        {
            var evaled = Evaluator.PartialEval(expression);
            var builder = new WhereExpressionBuilder();
            builder.Visit(evaled);
            return builder._current;
        }

        private static Expression StripQuotes(Expression e)
        {
            while (e.NodeType == ExpressionType.Quote)
            {
                e = ((UnaryExpression)e).Operand;
            }
            return e;
        }

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            if (m.Method.DeclaringType == typeof(Queryable) && m.Method.Name == "Where")
            {
                _sql.Append("SELECT * FROM (");
                Visit(m.Arguments[0]);
                _sql.Append(") AS T WHERE ");
                var lambda = (LambdaExpression)StripQuotes(m.Arguments[1]);
                Visit(lambda.Body);
                return m;
            }
            if (m.Method.DeclaringType == typeof(string) && m.Method.Name == "StartsWith")
            {
                _current.Operator = Operator.LikeStartsWith;
                var expression = StripQuotes(m.Arguments[0]);
                Visit(m.Object);
                Visit(expression);
                return m;
            }
            if (m.Method.DeclaringType == typeof(string) && m.Method.Name == "EndsWith")
            {
                _current.Operator = Operator.LikeEndsWith;
                var expression = StripQuotes(m.Arguments[0]);
                Visit(m.Object);
                Visit(expression);
                return m;
            }
            if (m.Method.DeclaringType == typeof(string) && m.Method.Name == "Contains")
            {
                _current.Operator = Operator.LikeContains;
                var expression = StripQuotes(m.Arguments[0]);
                Visit(m.Object);
                Visit(expression);
                return m;
            }
            if (m.Method.DeclaringType == typeof(Enumerable) && m.Method.Name == "Contains")
            {
                _current.Operator = Operator.In;
                var listExp = StripQuotes(m.Arguments[0]);
                var propExp = StripQuotes(m.Arguments[1]);
                Visit(propExp);
                Visit(listExp);
                return m;
            }
            if (m.Method.DeclaringType.IsGenericType && m.Method.DeclaringType.GetGenericTypeDefinition() == typeof(List<>) && m.Method.Name == "Contains")
            {
                _current.Operator = Operator.In;
                var expression = StripQuotes(m.Arguments[0]);
                Visit(expression);
                Visit(m.Object);
                return m;
            }

            throw new NotSupportedException(string.Format("The method '{0}' is not supported", m.Method.Name));
        }

        protected override Expression VisitUnary(UnaryExpression u)
        {
            switch (u.NodeType)
            {
                case ExpressionType.Not:
                    _sql.Append(" NOT ");
                    Visit(u.Operand);
                    break;
                case ExpressionType.Convert:
                    Visit(u.Operand);
                    break;
                default:
                    throw new NotSupportedException(string.Format("The unary operator '{0}' is not supported",
                        u.NodeType));
            }

            return u;
        }

        protected override Expression VisitBinary(BinaryExpression b)
        {
            if (_current == null)
                _current = new Where();

            _stack.Push(_current);
            _current = new Where();

            _sql.Append("(");
            Visit(b.Left);

            var tmp = _stack.Pop();
            tmp.SetOperand(_current);
            _current = tmp;

            switch (b.NodeType)
            {
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    _sql.Append(" AND ");
                    _current.Operator = Operator.And;
                    break;
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    _sql.Append(" OR");
                    _current.Operator = Operator.Or;
                    break;
                case ExpressionType.Equal:
                    _sql.Append(" = ");
                    _current.Operator = Operator.Equals;
                    break;
                case ExpressionType.NotEqual:
                    _sql.Append(" <> ");
                    _current.Operator = Operator.NotEquals;
                    break;
                case ExpressionType.LessThan:
                    _sql.Append(" < ");
                    _current.Operator = Operator.LessThan;
                    break;
                case ExpressionType.LessThanOrEqual:
                    _sql.Append(" <= ");
                    _current.Operator = Operator.LessThanOrEquals;
                    break;
                case ExpressionType.GreaterThan:
                    _sql.Append(" > ");
                    _current.Operator = Operator.GreaterThan;
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    _sql.Append(" >= ");
                    _current.Operator = Operator.GreaterThanOrEquals;
                    break;
                default:
                    throw new NotSupportedException(string.Format("The binary operator '{0}' is not supported",
                        b.NodeType));
            }

            _stack.Push(_current);
            _current = new Where();

            Visit(b.Right);
            _sql.Append(")");

            tmp = _stack.Pop();
            tmp.SetOperand(_current);
            _current = tmp;

            return b;
        }

        protected override Expression VisitConstant(ConstantExpression c)
        {
            var q = c.Value as IQueryable;

            if (q != null)
            {
                _sql.Append("SELECT * FROM ");
                _sql.Append(q.ElementType.Name);
            }
            else if (c.Value == null)
            {
                _sql.Append("NULL");
                _current.SetOperand(null);
            }
            else
            {
                switch (Type.GetTypeCode(c.Value.GetType()))
                {
                    case TypeCode.Boolean:
                        _sql.Append(((bool)c.Value) ? 1 : 0);
                        _current.SetOperand(c.Value);
                        break;

                    case TypeCode.String:
                        _sql.Append("'");
                        _sql.Append(c.Value);
                        _sql.Append("'");
                        _current.SetOperand(c.Value);
                        break;
                    case TypeCode.Object:
                        if (c.Value is IEnumerable && _current.Operator == Operator.In)
                        {
                            _current.SetOperand(c.Value);
                        }
                        else
                        {
                            throw new NotSupportedException(string.Format("The constant for '{0}' is not supported", c.Value));
                        }
                        break;
                    default:
                        _sql.Append(c.Value);
                        _current.SetOperand(c.Value);
                        break;
                }
            }

            return c;
        }

        protected override Expression VisitMember(MemberExpression m)
        {
            if (m.Expression != null && m.Expression.NodeType == ExpressionType.Parameter)
            {
                _sql.Append(m.Member.Name);
                _current.SetOperand(m.Member);
                return m;
            }

            throw new NotSupportedException(string.Format("The member '{0}' is not supported", m.Member.Name));
        }
    }
}