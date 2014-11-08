using Taga.SimpLinq.QueryBuilder;

namespace Taga.Core.Repository.SimpLinq
{
    public class MySqlSimpLinqResolver : SqlSimpLinqResolver
    {
        protected override void ResolvePage(ISelectQuery query)
        {
            if (query.Page == null)
            {
                return;
            }

            // LIMIT 10 OFFSET 0

            Sql.AppendLine()
                .AppendFormat("LIMIT ");

            AddParam("_limit_", query.Page.PageSize);

            Sql.Append(" OFFSET ");

            AddParam("_offset_", (query.Page.PageIndex - 1)*query.Page.PageSize);
        }
    }
}