using Taga.Orm.Meta;

namespace Taga.Orm.Sql.Command
{
    public interface ICommandMetaBuilder
    {
        CommandMeta BuildInsertCommandMeta(TableMeta table);
                    
        CommandMeta BuildUpdateCommandMeta(TableMeta table);
                    
        CommandMeta BuildDeleteCommandMeta(TableMeta table);
                    
        CommandMeta BuildGetByIdCommandMeta(TableMeta table);
    }
}