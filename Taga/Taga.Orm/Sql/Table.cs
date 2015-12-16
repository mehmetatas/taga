using Taga.Orm.Meta;

namespace Taga.Orm.Sql
{
    public class Table
    {
        public TableMeta Meta { get; set; }
        public string Alias { get; set; }

        public override string ToString()
        {
            return $"[{Meta.TableName}] {Alias}";
        }
    }
}