using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;

namespace Taga.Orm.Sql.Command
{
    public class Command
    {
        internal CommandType Type { get; private set; }
        public string CommandText { get; private set; }
        public IReadOnlyDictionary<string, CommandParameter> Parameters { get; private set; }

        private Command() { }

        public static Command TextCommand(string cmd, IDictionary<string, CommandParameter> parameters = null)
        {
            if (parameters == null)
            {
                parameters = new Dictionary<string, CommandParameter>();
            }

            return new Command
            {
                CommandText = cmd,
                Parameters = new ReadOnlyDictionary<string, CommandParameter>(parameters),
                Type = CommandType.Text
            };
        }

        public static Command SpCommand(string cmd, IDictionary<string, CommandParameter> parameters = null)
        {
            if (parameters == null)
            {
                parameters = new Dictionary<string, CommandParameter>();
            }

            return new Command
            {
                CommandText = cmd,
                Parameters = new ReadOnlyDictionary<string, CommandParameter>(parameters),
                Type = CommandType.StoredProcedure
            };
        }
    }
}