using System.Collections.Generic;
using System.Linq;
using Taga.Orm.Meta;

namespace Taga.Orm.Sql.Command
{
    class SimpleCommandBuilder : ISimpleCommandBuilder
    {
        private readonly TableMeta _tableMeta;
        private readonly CommandMeta _insertCommandMeta;
        private readonly CommandMeta _updateCommandMeta;
        private readonly CommandMeta _deleteCommandMeta;
        private readonly CommandMeta _getByIdCommandMeta;

        public SimpleCommandBuilder(TableMeta tableMeta)
        {
            _tableMeta = tableMeta;

            var builder = tableMeta.DbMeta.DbProvider.CreateCommandMetaBuilder(tableMeta.DbMeta);

            _insertCommandMeta = builder.BuildInsertCommandMeta(tableMeta);
            _updateCommandMeta = builder.BuildUpdateCommandMeta(tableMeta);
            _deleteCommandMeta = builder.BuildDeleteCommandMeta(tableMeta);
            _getByIdCommandMeta = builder.BuildGetByIdCommandMeta(tableMeta);
        }

        public Command BuildInsertCommand(object entity)
        {
            return _insertCommandMeta.CreateCommand(entity);
        }

        public Command BuildUpdateCommand(object entity)
        {
            return _updateCommandMeta.CreateCommand(entity);
        }

        public Command BuildDeleteCommand(object entity)
        {
            return _deleteCommandMeta.CreateCommand(entity);
        }

        public Command BuildGetByIdCommand(object id)
        {
            if (_tableMeta.Type == id.GetType())
            {
                return _getByIdCommandMeta.CreateCommand(id);
            }

            var paramMeta = _getByIdCommandMeta.ParameterMeta.First();
            return Command.TextCommand(_getByIdCommandMeta.CommandText, new Dictionary<string, CommandParameter>
            {
                {
                    paramMeta.Key, new CommandParameter
                    {
                        Value = id,
                        Name = paramMeta.Key,
                        ParameterMeta = paramMeta.Value.ParameterMeta
                    }
                }
            });
        }
    }
}