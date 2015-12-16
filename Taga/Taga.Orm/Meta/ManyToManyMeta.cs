using System;
using System.Collections;
using Taga.Orm.Dynamix;

namespace Taga.Orm.Meta
{
    public class ManyToManyMeta : IAssociationMeta
    {
        public ManyToManyMeta(IDbMeta dbMeta)
        {
            DbMeta = dbMeta;
        }

        public IDbMeta DbMeta { get; private set; }

        public Func<IList> ListFactory { get; set; }
        public IGetterSetter ListGetterSetter { get; set; }
        public ColumnMeta ParentColumn { get; set; }
        public ColumnMeta ChildColumn { get; set; }
        public IAssociationLoader Loader { get; set; }

        public override string ToString()
        {
            return ParentColumn.Table.ToString();
        }
    }
}