using System.Data;
using Taga.Orm.Meta;
using Taga.Orm.Sql.Command;
using Taga.Orm.Sql.Delete;
using Taga.Orm.Sql.Select;
using Taga.Orm.Sql.Where;

namespace Taga.Orm.Providers.SqlServer2012
{
    public abstract class SqlServer2012Provider : IDbProvider
    {
        public virtual char QuoteOpen => '[';

        public virtual char QuoteClose => ']';

        public virtual char ParameterPrefix => '@';

        public virtual ISelectCommandBuilder CreateSelectCommandBuilder(IDbMeta meta)
        {
            return new SqlServer2012SelectCommandBuilder(meta);
        }

        public virtual IWhereCommandBuilder CreateWhereCommandBuilder(IDbMeta meta)
        {
            return new SqlServer2012WhereCommandBuilder(this);
        }

        public virtual ICommandMetaBuilder CreateCommandMetaBuilder(IDbMeta meta)
        {
            return new SqlServer2012CommandMetaBuilder();
        }

        public virtual IDeleteManyCommandBuilder CreateDeleteManyCommandBuilder(IDbMeta meta)
        {
            return new Sql2012DeleteWhereCommandBuilder(meta);
        }
        
        public abstract IDbConnection CreateConnection();
    }
}
