using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Taga.Core.IoC;
using Taga.Core.Repository.Command;
using Taga.Core.Repository.Mapping;
using Taga.SimpLinq.QueryBuilder;

namespace Taga.Core.Repository.SimpLinq
{
    public abstract class SqlSimpLinqResolver : ISimpLinqResolver
    {
        private readonly DatabaseMapping _dbMapping;

        protected readonly StringBuilder Sql = new StringBuilder();
        protected readonly IDictionary<string, object> Parameters = new Dictionary<string, object>();
        private readonly char _parameterIdentifier;

        protected SqlSimpLinqResolver(char parameterIdentifier)
        {
            _parameterIdentifier = parameterIdentifier;
            var prov = ServiceProvider.Provider.GetOrCreate<IMappingProvider>();
            _dbMapping = prov.GetDatabaseMapping();

            Sql = new StringBuilder();
            Parameters = new Dictionary<string, object>();
        }

        protected string Tbl(Type type)
        {
            return _dbMapping.GetTableName(type);
        }

        protected string Tbl(PropertyInfo propInf)
        {
            return Tbl(propInf.DeclaringType);
        }

        protected string Col(PropertyInfo propInf)
        {
            return _dbMapping.GetColumnName(propInf);
        }

        protected string FullDb(PropertyInfo propInf)
        {
            return String.Format("{0}.{1}",
                Tbl(propInf),
                Col(propInf));
        }

        protected string FullClr(PropertyInfo propInf)
        {
            return String.Format("{0}_{1}",
                Tbl(propInf),
                Col(propInf));
        }

        protected void AddParam(PropertyInfo propInf, object value)
        {
            AddParam(FullClr(propInf), value);
        }

        protected void AddParam(string name, object value)
        {
            var paramName = String.Format("p_{0}", name);

            if (Parameters.ContainsKey(paramName))
            {
                var count = Parameters.Keys.Count(k => k.StartsWith(paramName));
                paramName = String.Format("{0}_{1}", paramName, count);
            }

            Sql.AppendFormat("{0}{1}", _parameterIdentifier, paramName);
            Parameters.Add(paramName, value);
        }

        public virtual ICommand Resolve(ISelectQuery query)
        {
            ClearBuffer();
            BeginSelect(query);
            ResolveJoin(query);
            ResolveWhere(query);
            ResolveOrderBy(query);
            ResolvePage(query);
            return BuildSql();
        }

        protected virtual void ClearBuffer()
        {
            Parameters.Clear();
            Sql.Clear();
        }

        protected virtual void BeginSelect(ISelectQuery query)
        {
            Sql.Append("SELECT")
                .AppendLine()
                .Append("    ")
                .Append(String.Join(String.Format(",{0}    ", Environment.NewLine),
                    query.SelectProperties.Select(pi => String.Format("{0} AS {1}", FullDb(pi), FullClr(pi)))))
                .AppendLine()
                .Append("FROM ")
                .Append(Tbl(query.FromType));
        }

        protected virtual void ResolveJoin(ISelectQuery query)
        {
            if (!query.JoinProperties.Any())
            {
                return;
            }

            Sql.AppendLine()
                .Append(String.Join(Environment.NewLine,
                    query.JoinProperties.Select(pi => String.Format("    {0} JOIN {1} ON {2} = {3}",
                        pi.JoinType == JoinType.Inner ? "INNER" : (pi.JoinType == JoinType.Left ? "LEFT" : "RIGHT"),
                        Tbl(pi.RightProperty),
                        FullDb(pi.RightProperty),
                        FullDb(pi.LeftProperty)))));
        }

        protected virtual void ResolveWhere(ISelectQuery query)
        {
            if (query.Where == null)
            {
                return;
            }

            Sql.AppendLine()
                .Append("WHERE")
                .AppendLine();

            ResolveWhereRecursive(query.Where);
        }

        protected virtual void ResolveWhereRecursive(IWhere where)
        {
            switch (where.Operator)
            {
                case Operator.Or:
                case Operator.And:
                    ResolveWhereLogical(where);
                    break;
                case Operator.Equals:
                case Operator.NotEquals:
                    ResolveWhereEquality(where);
                    break;
                case Operator.GreaterThanOrEquals:
                case Operator.GreaterThan:
                case Operator.LessThanOrEquals:
                case Operator.LessThan:
                    ResolveWhereComparison(where);
                    break;
                case Operator.LikeContains:
                case Operator.LikeStartsWith:
                case Operator.LikeEndsWith:
                case Operator.NotLikeContains:
                case Operator.NotLikeStartsWith:
                case Operator.NotLikeEndsWith:
                    ResolveWhereLike(where);
                    break;
                case Operator.In:
                case Operator.NotIn:
                    ResolveWhereIn(where);
                    break;
                default:
                    throw new NotSupportedException("Unsupported operator: " + where.Operator);
            }
        }

        protected virtual void ResolveWhereLogical(IWhere where)
        {
            Sql.Append("(");
            ResolveWhereRecursive((IWhere)where.Operand1);
            Sql.Append(")")
                .Append(" ")
                .Append(GetOperator(where.Operator))
                .Append(" ")
                .Append("(");
            ResolveWhereRecursive((IWhere)where.Operand2);
            Sql.Append(")");
        }

        protected virtual void ResolveWhereEquality(IWhere where)
        {
            var propInf = (PropertyInfo)where.Operand1;
            var operand2 = where.Operand2;

            var isNotEquals = where.Operator == Operator.NotEquals;

            Sql.Append(FullDb(propInf))
                .Append(" ");
            if (operand2 == null)
            {
                Sql.AppendFormat(" IS {0}NULL", isNotEquals ? "NOT " : String.Empty);
            }
            else
            {
                Sql.AppendFormat(" {0} ", isNotEquals ? "<>" : "=");
                AddParam(propInf, operand2);
            }
        }

        protected virtual void ResolveWhereComparison(IWhere where)
        {
            var propInf = (PropertyInfo)where.Operand1;

            Sql.Append(FullDb(propInf))
                .Append(" ")
                .Append(GetOperator(where.Operator))
                .Append(" ");
            AddParam(propInf, where.Operand2);
        }

        protected virtual void ResolveWhereLike(IWhere where)
        {
            var oper = where.Operator;
            var propInf = (PropertyInfo)where.Operand1;

            var arg0 = oper == Operator.LikeStartsWith || oper == Operator.NotLikeStartsWith
                ? String.Empty
                : "%";

            var arg2 = oper == Operator.LikeEndsWith || oper == Operator.NotLikeEndsWith
                ? String.Empty
                : "%";

            var isNotLike = oper == Operator.NotLikeContains ||
                            oper == Operator.NotLikeStartsWith ||
                            oper == Operator.NotLikeEndsWith;

            Sql.Append(FullDb(propInf));

            if (isNotLike)
            {
                Sql.Append(" NOT");
            }

            Sql.Append(" LIKE ");

            AddParam(propInf, String.Format("{0}{1}{2}", arg0, where.Operand2, arg2));
        }

        protected virtual void ResolveWhereIn(IWhere where)
        {
            var propInf = (PropertyInfo)where.Operand1;

            var isNotIn = where.Operator == Operator.NotIn;

            Sql.Append(FullDb(propInf));

            if (isNotIn)
            {
                Sql.Append(" NOT");
            }

            Sql.Append(" IN (");

            var list = (IList)where.Operand2;

            var comma = String.Empty;
            for (var i = 0; i < list.Count; i++)
            {
                Sql.Append(comma);
                comma = ",";
                AddParam(propInf, list[i]);
            }

            Sql.Append(")");
        }

        protected virtual void ResolveOrderBy(ISelectQuery query)
        {
            if (!query.OrderByProperties.Any())
            {
                return;
            }

            Sql.AppendLine()
                .Append("ORDER BY")
                .AppendLine()
                .Append("    ")
                .Append(String.Join(",", query.OrderByProperties.Select(ob => String.Format("{0} {1}",
                    FullDb(ob.OrderProperty),
                    ob.Descending ? "DESC" : "ASC"))));
        }

        protected abstract void ResolvePage(ISelectQuery query);

        protected virtual ICommand BuildSql()
        {
            var parameters = Parameters
                .Select(kv => (ICommandParameter) new CommandParameter(_parameterIdentifier, kv.Key, kv.Value))
                .ToArray();

            return new Command.Command(Sql.ToString(), parameters, true);
        }

        protected static string GetOperator(Operator oper)
        {
            switch (oper)
            {
                case Operator.And:
                    return "AND";
                case Operator.Or:
                    return "OR";
                case Operator.Equals:
                    return "==";
                case Operator.NotEquals:
                    return "<>";
                case Operator.GreaterThan:
                    return ">";
                case Operator.LessThan:
                    return "<";
                case Operator.GreaterThanOrEquals:
                    return ">=";
                case Operator.LessThanOrEquals:
                    return "<=";
                case Operator.In:
                    return "IN";
                case Operator.LikeStartsWith:
                case Operator.LikeEndsWith:
                case Operator.LikeContains:
                    return "LIKE";
                default:
                    throw new NotSupportedException("Unsupported operator: " + oper);
            }
        }
    }
}
