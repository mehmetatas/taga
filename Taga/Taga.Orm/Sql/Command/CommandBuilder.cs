using System.Collections.Generic;
using System.Data;
using System.Text;
using Taga.Orm.Meta;

namespace Taga.Orm.Sql.Command
{
    public class CommandBuilder
    {
        private readonly StringBuilder _cmd;
        private readonly IDictionary<string, CommandParameter> _params;

        public CommandBuilder()
        {
            _cmd = new StringBuilder();
            _params = new Dictionary<string, CommandParameter>();
        }

        public CommandBuilder Append(string sql)
        {
            _cmd.Append(sql);
            return this;
        }

        public CommandBuilder AppendFormat(string format, params object[] args)
        {
            _cmd.AppendFormat(format, args);
            return this;
        }

        public CommandBuilder AddParameter(CommandParameter param)
        {
            _params.Add(param.Name, param);
            return this;
        }

        public CommandBuilder AddParameter(string name, object value, ParameterMeta meta = null)
        {
            if (meta == null)
            {
                meta = new ParameterMeta
                {
                    DbType = value?.GetType().GetDbType() ?? DbType.Object
                };
            }

            return AddParameter(new CommandParameter
            {
                Name = name,
                Value = value,
                ParameterMeta = meta
            });
        }

        public Command Build()
        {
            return Command.TextCommand(_cmd.ToString().TrimEnd(), _params);
        }
    }
}
