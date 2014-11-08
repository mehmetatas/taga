using System.Linq;

namespace Taga.Core.Repository.Command
{
    public interface ICommand
    {
        string CommandText { get; }
        ICommandParameter[] Parameters { get; }
        bool IsRawSql { get; }
    }

    public static class CommandExtensions
    {
        public static object GetParameterValue(this ICommand cmd, string paramName)
        {
            return cmd.Parameters.First(p => p.Name == paramName).Value;
        }
    }
}