namespace Taga.Core.Repository.Command
{
    public interface ICommandParameter
    {
        char ParamIdentifier { get; }
        string Name { get; }
        object Value { get; }
    }
}