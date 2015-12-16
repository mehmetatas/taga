using Taga.Orm.Meta;

namespace Taga.Orm.Sql
{
    public class Column
    {
        public ColumnMeta Meta { get; set; }
        public string Alias { get; set; }
        public Table Table { get; set; }

        public override string ToString()
        {
            return $"[{Meta.Table.TableName}] {Table.Alias}.{Alias}";
        }
    }
}