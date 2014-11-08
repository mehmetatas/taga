using Taga.SimpLinq.QueryBuilder;

namespace Taga.Core.Repository.SimpLinq
{
    public class OracleSimpleLinqResolver : SqlSimpLinqResolver
    {
        protected override void ResolvePage(ISelectQuery query)
        {
            if (query.Page == null)
            {
                return;
            }

            // select * from (select rownum as rownum_, * from table) t
            // where rownum_ between 1 and 10;

            Sql.Insert(7, "ROWNUM AS ROWNUM_, ");
            Sql.Insert(0, "SELECT * FROM (");
            Sql.Append(") T WHERE ROWNUM_ BETWEEN ");

            AddParam("_rownum_start", (query.Page.PageIndex - 1)*query.Page.PageSize + 1);
            Sql.Append(" AND ");
            AddParam("_rownum_end", query.Page.PageSize*query.Page.PageIndex);
        }
    }
}