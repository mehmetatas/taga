using System.Data;

namespace Taga.Orm.Meta
{
    public class ParameterMeta
    {
        public DbType DbType { get; set; }
        public byte Scale { get; set; }
        public byte Precision { get; set; }
        public int Size { get; set; }
    }
}