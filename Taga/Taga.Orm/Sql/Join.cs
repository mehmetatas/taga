namespace Taga.Orm.Sql
{
    public class Join
    {
        public Column RightColumn { get; set; }
        public Column LeftColumn { get; set; }
        public JoinType Type { get; set; }
    }
}