using System;

namespace Taga.Orm.Sql
{
    public enum Operator
    {
        Not = 0,
        And = 1,
        Equals,
        GreaterThan,
        LessThan,
        In,
        LikeStartsWith,
        LikeEndsWith,
        LikeContains,
        // Nots
        Or = ~And,
        NotEquals = ~Equals,
        GreaterThanOrEquals = ~LessThan,
        LessThanOrEquals = ~GreaterThan,
        NotIn = ~In,
        NotLikeStartsWith = ~LikeStartsWith,
        NotLikeEndsWith = ~LikeEndsWith,
        NotLikeContains = ~LikeContains
    }

    public static class OperatorExtensions
    {
        public static string GetOperator(this Operator sqlOperator)
        {
            switch (sqlOperator)
            {
                case Operator.Not:
                    return "NOT";
                case Operator.And:
                    return "AND";
                case Operator.Equals:
                    return "=";
                case Operator.GreaterThan:
                    return ">";
                case Operator.LessThan:
                    return "<";
                case Operator.In:
                    return "IN";
                case Operator.LikeStartsWith:
                case Operator.LikeEndsWith:
                case Operator.LikeContains:
                    return "LIKE";
                case Operator.Or:
                    return "OR";
                case Operator.NotEquals:
                    return "<>";
                case Operator.GreaterThanOrEquals:
                    return ">=";
                case Operator.LessThanOrEquals:
                    return "<=";
                case Operator.NotIn:
                    return "NOT IN";
                case Operator.NotLikeStartsWith:
                case Operator.NotLikeEndsWith:
                case Operator.NotLikeContains:
                    return "NOT LIKE";
                default:
                    throw new ArgumentOutOfRangeException(nameof(sqlOperator));
            }
        }
    }
}
