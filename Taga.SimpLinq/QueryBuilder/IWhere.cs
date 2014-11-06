
namespace Taga.SimpLinq.QueryBuilder
{
    public interface IWhere
    {
        object Operand1 { get; }
        object Operand2 { get; }
        Operator Operator { get; }
    }
}