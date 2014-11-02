
namespace Taga.Core.Repository.Command
{
    public class Command : ICommand
    {
        public Command(string commandText, ICommandParameter[] parameters, bool isRawSql)
        {
            CommandText = commandText;
            Parameters = parameters;
            IsRawSql = isRawSql;
        }

        public string CommandText { get; private set; }
        public ICommandParameter[] Parameters { get; private set; }
        public bool IsRawSql { get; private set; }
    }
}