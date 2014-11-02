using System;
using System.Collections.Generic;
using System.Linq;
using Taga.Core.IoC;
using Taga.Core.Repository.Command.Builders;
using Taga.Core.Repository.Mapping;

namespace Taga.Core.Repository.Command
{
    public class CommandBuilder : ICommandBuilder
    {
        private readonly char _parameterIdentifier;

        public CommandBuilder(char parameterIdentifier)
        {
            _parameterIdentifier = parameterIdentifier;
        }

        public ICommand BuildCommand(string spNameOrSql, IDictionary<string, object> args = null, bool rawSql = false)
        {
            var parameters = new List<ICommandParameter>();

            var hasArgs = args != null && args.Any();
            if (hasArgs)
            {
                BuildParams(ref spNameOrSql, args, parameters);
            }

            return new Command(spNameOrSql, parameters.ToArray(), rawSql);
        }

        private void BuildParams(ref string spNameOrSql, IDictionary<string, object> args, List<ICommandParameter> parameters)
        {
            var i = 0;
            foreach (var arg in args)
            {
                var paramName = String.Format("p{0}", i++);

                spNameOrSql = spNameOrSql.Replace(
                    String.Format("~{0}", arg.Key),
                    String.Format("{0}{1}", _parameterIdentifier, paramName));

                parameters.Add(new CommandParameter(_parameterIdentifier, paramName, arg.Value));
            }
        }

        public static ICommandBuilder CreateBuilder()
        {
            var db = ServiceProvider.Provider.GetOrCreate<IMappingProvider>().GetDatabaseMapping().DbSystem;

            switch (db)
            {
                case DbSystem.SqlServer:
                    return new SqlServerCommandBuilder();
                case DbSystem.MySql:
                    return new MySqlCommandBuilder();
                case DbSystem.Oracle:
                    return new OracleCommandBuilder();
                default:
                    throw new NotSupportedException("Unsupported db system: " + db);
            }
        }
    }
}