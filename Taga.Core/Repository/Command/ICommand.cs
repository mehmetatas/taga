
namespace Taga.Core.Repository.Command
{
    public interface ICommand
    {
        string CommandText { get; }
        ICommandParameter[] Parameters { get; }
        bool IsRawSql { get; }
    }
}