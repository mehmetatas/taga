namespace Taga.Orm.Sql.Select
{
    public interface ISelectCommandBuilder
    {
        Command.Command Build(ISelectQuery query);
    }
}
