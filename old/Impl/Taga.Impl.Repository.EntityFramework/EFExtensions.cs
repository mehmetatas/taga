using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;

namespace Taga.Impl.Repository.EntityFramework
{
    static class EFExtensions
    {
        public static ObjectContext GetObjectContext(this DbContext ctx)
        {
            return (ctx as IObjectContextAdapter).ObjectContext;
        }
    }
}
