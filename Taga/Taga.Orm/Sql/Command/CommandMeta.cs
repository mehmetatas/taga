using System.Collections.Generic;
using Taga.Orm.Meta;

namespace Taga.Orm.Sql.Command
{
    public class CommandMeta
    {
        public string CommandText { get; set; }
        public IDictionary<string, ColumnMeta> ParameterMeta { get; set; }

        public Command CreateCommand(object entity)
        {
            var parameters = new Dictionary<string, CommandParameter>();

            foreach (var kv in ParameterMeta)
            {
                var paramName = kv.Key;
                var colMeta = kv.Value;

                var value = colMeta.GetterSetter.Get(entity);

                if (colMeta.IsRefrence && value != null)
                {
                    value = colMeta.ReferencedTable.IdColumn.GetterSetter.Get(value);
                }

                parameters.Add(paramName, new CommandParameter
                {
                    Name = paramName,
                    Value = value,
                    ParameterMeta = colMeta.ParameterMeta
                });
            }

            return Command.TextCommand(CommandText, parameters);
        }
    }
}