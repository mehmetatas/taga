using Taga.Orm.Sql.Where.ExpressionVisitors;

namespace Taga.Orm.Providers.SqlServer2012
{
    public class SqlServer2012WhereCommandBuilder : WhereCommandBuilder
    {
        public SqlServer2012WhereCommandBuilder(IDbProvider provider) : base(provider)
        {
        }
    }
}
