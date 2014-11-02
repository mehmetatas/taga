using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.AccessControl;

namespace Taga.Repository.NH
{
    public interface INHSpCallBuilder
    {
        string BuildSpCall(string spNameOrSql, IDictionary<string, object> args = null);
    }

    public interface ICommandBuilder
    {
        /**
         * NHMySql
         * NHSqlServer
         * NHOracle
         * 
         * EFSqlServer
         * 
         * HybridMySql
         * HybridSqlServer
         * HybridOracle
         */
        Command BuildCommand(string spNameOrSql, IDictionary<string, object> args = null, bool rawSql = false);
    }

    public class Command
    {
        public Command(string commandText, CommandParameter[] parameters, bool isRawSql)
        {
            CommandText = commandText;
            Parameters = parameters;
            IsRawSql = isRawSql;
        }

        public string CommandText { get; private set; }
        public CommandParameter[] Parameters { get; private set; }
        public bool IsRawSql { get; private set; }

        public IDbCommand ToDbCommand(IDbConnection conn, IDbTransaction tran = null,
            bool addParametersWithIdentifier = false)
        {
            var cmd = conn.CreateCommand();
            cmd.Transaction = tran;

            cmd.CommandText = CommandText;
            cmd.CommandType = IsRawSql
                ? CommandType.Text
                : CommandType.StoredProcedure;

            if (Parameters != null)
            {
                foreach (var parameter in Parameters)
                {
                    var param = cmd.CreateParameter();

                    param.ParameterName = addParametersWithIdentifier
                        ? String.Format("{0}{1}", parameter.ParamIdentifier, parameter.Name)
                        : parameter.Name;

                    param.Value = parameter.Value;

                    cmd.Parameters.Add(param);
                }
            }

            return cmd;
        }
    }

    public class CommandParameter
    {
        public CommandParameter(char paramIdentifier, string name, object value)
        {
            ParamIdentifier = paramIdentifier;
            Name = name;
            Value = value;
        }

        public char ParamIdentifier { get; private set; }
        public string Name { get; private set; }
        public object Value { get; private set; }
    }

    public class EFSqlServerCommandBuilder : ICommandBuilder
    {
        public Command BuildCommand(string spNameOrSql, IDictionary<string, object> args = null, bool rawSql = false)
        {
            var hasArgs = args != null && args.Any();

            var parameters = new List<CommandParameter>();

            if (hasArgs)
            {
                var i = 0;
                foreach (var arg in args)
                {
                    var paramName = String.Format("p{0}", i);
                    spNameOrSql = spNameOrSql.Replace(arg.Key, String.Format("@{0}", paramName));
                    parameters.Add(new CommandParameter('@', paramName, arg.Value));
                }
            }

            return new Command(spNameOrSql, parameters.ToArray(), rawSql);
        }
    }
}
