using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Taga.SimpLinq.QueryBuilder.Impl
{
    class Where : IWhere
    {
        public Operator Operator { get; set; }
        public object Operand1 { get; private set; }
        public object Operand2 { get; private set; }

        public void SetOperand(object operand)
        {
            if (Operand1 == null)
            {
                Operand1 = operand;
            }
            else if (Operand2 == null)
            {
                Operand2 = operand;
            }
            else
            {
                throw new Exception("All operands already set!");
            }
        }

        private bool IsNull
        {
            get { return Operand1 == null && Operand2 == null; }
        }

        public override string ToString()
        {
            if (Operator == Operator.None)
            {
                if (Operand1 is PropertyInfo)
                {
                    return FullName();
                }
                if (Operand1 == null)
                {
                    return "NULL";
                }
                return Convert.ToString(Operand1);
            }

            if (Operator == Operator.And || Operator == Operator.Or)
            {
                return String.Format("({0}) {1} ({2})", Operand1, GetOperator(), Operand2);
            }

            if (Operator == Operator.LessThan || Operator == Operator.LessThanOrEquals ||
                Operator == Operator.GreaterThan || Operator == Operator.GreaterThanOrEquals)
            {
                return String.Format("{0} {1} {2}", Operand1, GetOperator(), Operand2);
            }

            if (Operator == Operator.Not)
            {
                return String.Format("NOT {0}", Operand1);
            }

            if (Operator == Operator.Equals)
            {
                if (Operand2 is Where && ((Where)Operand2).IsNull)
                {
                    return String.Format("{0} IS {1}", Operand1, Operand2);
                }
                return String.Format("{0} = {1}", Operand1, Operand2);
            }

            if (Operator == Operator.NotEquals)
            {
                if (Operand2 is Where && ((Where)Operand2).IsNull)
                {
                    return String.Format("{0} IS NOT {1}", Operand1, Operand2);
                }
                return String.Format("{0} <> {1}", Operand1, Operand2);
            }

            if (Operator == Operator.LikeContains)
            {
                return String.Format("{0} LIKE '%{1}%'", FullName(), Operand2);
            }
            if (Operator == Operator.LikeStartsWith)
            {
                return String.Format("{0} LIKE '{1}%'", FullName(), Operand2);
            }
            if (Operator == Operator.LikeEndsWith)
            {
                return String.Format("{0} LIKE '%{1}'", FullName(), Operand2);
            }

            return String.Format("{0} IN ({1})", FullName(), String.Join(",", ((IEnumerable)Operand2).Cast<object>()));
        }

        private string FullName()
        {
            var propInf = (PropertyInfo)Operand1;
            return String.Format("{0}.{1}", propInf.DeclaringType.Name, propInf.Name);
        }

        private string GetOperator()
        {
            switch (Operator)
            {
                case Operator.And:
                    return "AND";
                case Operator.Or:
                    return "OR";
                case Operator.Not:
                    return "NOT";
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
                    throw new ArgumentOutOfRangeException("Operator");
            }
        }
    }
}