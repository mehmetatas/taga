using Taga.SimpLinq.QueryBuilder;

namespace Taga.Core.Repository.SimpLinq
{
    public class SqlServerSimpLinqResolver : SqlSimpLinqResolver
    {
        protected override void ResolvePage(ISelectQuery query)
        {
            if (query.Page == null)
            {
                return;
            }

            // OFFSET 0 ROWS FETCH FIRST 10 ROWS

            Sql.AppendLine()
                .AppendFormat("OFFSET ");

            AddParam("_offset_", (query.Page.PageIndex - 1) * query.Page.PageSize);

            Sql.Append(" ROWS FETCH FIRST ");

            AddParam("_fetch_", query.Page.PageSize);

            Sql.Append(" ROWS ONLY ");
        }
    }
}

