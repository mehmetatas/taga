
namespace Taga.SimpLinq.QueryBuilder
{
    public interface IWhere
    {
        object Operand1 { get; }
        object Operand2 { get; }
        Operator Operator { get; }
    }

    public static class WhereExtensions
    {
        public static bool IsNull(this IWhere where)
        {
            return where.Operand1 == null && where.Operand2 == null;
        }
    }
}