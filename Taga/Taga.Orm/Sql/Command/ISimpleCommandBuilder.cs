namespace Taga.Orm.Sql.Command
{
    public interface ISimpleCommandBuilder
    {
        Command BuildInsertCommand(object entity);
        
        Command BuildDeleteCommand(object entity);
        
        Command BuildUpdateCommand(object entity);

        Command BuildGetByIdCommand(object id);
    }
}