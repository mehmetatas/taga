using System;
using System.Collections;
using Taga.Orm.Dynamix;

namespace Taga.Orm.Meta
{
    public class OneToManyMeta : IAssociationMeta
    {
        public OneToManyMeta(IDbMeta dbMeta)
        {
            DbMeta = dbMeta;
        }

        public IDbMeta DbMeta { get; private set; }

        public Func<IList> ListFactory { get; set; }
        public IGetterSetter ListGetterSetter { get; set; }
        public ColumnMeta ForeignKey { get; set; }
        public ColumnMeta PrimaryKey { get; set; }
        public IAssociationLoader Loader { get; set; }
    }
}