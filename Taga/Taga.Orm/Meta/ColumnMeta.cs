using System.Reflection;
using Taga.Orm.Dynamix;

namespace Taga.Orm.Meta
{
    public class ColumnMeta
    {
        public ColumnMeta(IDbMeta dbMeta)
        {
            DbMeta = dbMeta;
        }

        public IDbMeta DbMeta { get; private set; }

        public TableMeta Table { get; set; }
        public string ColumnName { get; set; }
        public bool Identity { get; set; }
        public bool AutoIncrement { get; set; }
        public bool IsRefrence { get; set; }
        public PropertyInfo Property { get; set; }
        public ParameterMeta ParameterMeta { get; set; }
        public TableMeta ReferencedTable { get; set; }
        public IGetterSetter GetterSetter { get; set; }
        public IAssociationLoader Loader { get; set; }

        public override string ToString()
        {
            return $"{Table}.{ColumnName}";
        }
    }
}