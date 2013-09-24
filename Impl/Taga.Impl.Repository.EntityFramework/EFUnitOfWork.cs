using System.Data.Entity;
using Taga.Core.Repository.Base;

namespace Taga.Impl.Repository.EntityFramework
{
    public class EFUnitOfWork : UnitOfWork
    {
        public EFUnitOfWork(DbContext dbContext)
        {
            DbContext = dbContext;
        }

        public override void Save()
        {
            DbContext.SaveChanges();
        }

        protected override void OnDispose()
        {
            base.OnDispose();

            DbContext.Dispose();
            DbContext = null;
        }

        internal DbContext DbContext { get; private set; }
    }
}
