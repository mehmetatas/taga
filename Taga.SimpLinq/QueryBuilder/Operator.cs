
namespace Taga.SimpLinq.QueryBuilder
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
}
