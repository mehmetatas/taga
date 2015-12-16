using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Taga.Orm.Providers;
using Taga.Orm.Sql.Command;
using Taga.Orm.Sql.Where.Expressions;

namespace Taga.Orm.Sql.Where.ExpressionVisitors
{
    /// <summary>
    /// Visits on IWhereExpressions
    /// Builds Command
    /// </summary>
    public abstract class WhereCommandBuilder : IWhereCommandBuilder
    {
        private readonly IDbProvider _provider;
        private readonly StringBuilder _sql = new StringBuilder();
        private readonly IDictionary<string, CommandParameter> _parameters = new Dictionary<string, CommandParameter>();

        protected WhereCommandBuilder(IDbProvider provider)
        {
            _provider = provider;
        }

        public virtual void Visit(LogicalExpression e)
        {
            _sql.Append("(");
            e.Operand1.Accept(this);
            _sql.Append(" ").Append(e.Operator.GetOperator()).Append(" ");
            e.Operand2.Accept(this);
            _sql.Append(")");
        }

        public virtual void Visit(BinaryExpression e)
        {
            var left = e.Operand1;
            var right = e.Operand2;

            var isNull = false;

            if (e.Operand1 is NullExpression)
            {
                left = e.Operand2;
                right = e.Operand1;
                isNull = true;
            }
            else if (e.Operand2 is NullExpression)
            {
                isNull = true;
            }

            left.Accept(this);

            if (isNull)
            {
                _sql.Append(" IS");
                if (e.Operator == Operator.NotEquals)
                {
                    _sql.Append(" NOT");
                }
            }
            else
            {
                _sql.Append(" ").Append(e.Operator.GetOperator()).Append(" ");
            }

            right.Accept(this);
        }

        public virtual void Visit(ColumnExpression e)
        {
            if (String.IsNullOrWhiteSpace(e.Column.Table.Alias))
            {
                _sql.Append(e.Column.Meta.ColumnName);
            }
            else
            {
                _sql.AppendFormat("{0}.{1}", e.Column.Table.Alias, e.Column.Meta.ColumnName);
            }
        }

        public virtual void Visit(ValueExpression e)
        {
            if (e.Value == null)
            {
                throw new NotSupportedException("Null values should be handled by NullExpression");
            }

            var paramName = $"wp{_parameters.Count}";
            _sql.AppendFormat("{0}{1}", _provider.ParameterPrefix, paramName);
            _parameters.Add(paramName, new CommandParameter
            {
                Name = paramName,
                Value = e.Value,
                ParameterMeta = e.ColumnMeta.ParameterMeta
            });
        }

        public virtual void Visit(NullExpression e)
        {
            _sql.Append(" NULL");
        }

        public virtual void Visit(NotExpression e)
        {
            _sql.Append(" NOT (");
            e.Operand.Accept(this);
            _sql.Append(")");
        }

        public virtual void Visit(LikeExpression e)
        {
            e.Column.Accept(this);

            switch (e.Operator)
            {
                case Operator.LikeStartsWith:
                case Operator.LikeContains:
                case Operator.LikeEndsWith:
                    _sql.Append(" LIKE ");
                    break;
                case Operator.NotLikeStartsWith:
                case Operator.NotLikeContains:
                case Operator.NotLikeEndsWith:
                    _sql.Append(" NOT LIKE ");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var value = (string)e.Value.Value;

            switch (e.Operator)
            {
                case Operator.LikeStartsWith:
                case Operator.NotLikeStartsWith:
                    value = value + "%";
                    break;
                case Operator.LikeEndsWith:
                case Operator.NotLikeEndsWith:
                    value = "%" + value;
                    break;
                case Operator.LikeContains:
                case Operator.NotLikeContains:
                    value = "%" + value + "%";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            e.Value.Value = value;

            Visit(e.Value);
        }

        public virtual void Visit(InExpression e)
        {
            var values = (IEnumerable)e.Values.Value;

            var iter = values.GetEnumerator();

            if (!iter.MoveNext())
            {
                _sql.Append("1=1");
                return;
            }

            e.Column.Accept(this);

            _sql.Append(" IN (");
            var comma = "";
            
            do
            {
                _sql.Append(comma);

                Visit(new ValueExpression
                {
                    Value = iter.Current,
                    ColumnMeta = e.Values.ColumnMeta
                });

                comma = ",";
            } while (iter.MoveNext());

            _sql.Append(")");
        }

        public Command.Command Build()
        {
            return Command.Command.TextCommand(_sql.ToString(), _parameters);
        }
    }
}