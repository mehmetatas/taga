using System.Data;
using Taga.Orm.Meta;
using Taga.Orm.Sql.Command;
using Taga.Orm.Sql.Delete;
using Taga.Orm.Sql.Select;
using Taga.Orm.Sql.Where;

namespace Taga.Orm.Providers
{
    public interface IDbProvider
    {
        char QuoteOpen { get; }

        char QuoteClose { get; }

        char ParameterPrefix { get; }

        ISelectCommandBuilder CreateSelectCommandBuilder(IDbMeta meta);

        IWhereCommandBuilder CreateWhereCommandBuilder(IDbMeta meta);

        ICommandMetaBuilder CreateCommandMetaBuilder(IDbMeta meta);

        IDeleteManyCommandBuilder CreateDeleteManyCommandBuilder(IDbMeta meta);
        
        IDbConnection CreateConnection();
    }
}
