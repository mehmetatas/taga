using System;
using Taga.Orm.Sql.Command;

namespace Taga.Orm.Meta
{
    public class TableMeta
    {
        public TableMeta(IDbMeta dbMeta)
        {
            DbMeta = dbMeta;
        }

        public IDbMeta DbMeta { get; private set; }

        public Type Type { get; set; }
        public ColumnMeta[] Columns { get; set; }
        public string TableName { get; set; }
        public bool AssociationTable { get; set; }
        public ColumnMeta IdColumn { get; set; }
        public ISimpleCommandBuilder SimpleCommandBuilder { get; set; }

        public Func<object> Factory { get; set; }

        public override string ToString()
        {
            return TableName;
        }
    }
}