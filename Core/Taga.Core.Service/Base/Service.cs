using System.Data;
using Taga.Core.Repository;

namespace Taga.Core.Service.Base
{
    public abstract class Service : IService
    {
        private readonly IUnitOfWork _uow;

        protected Service(IUnitOfWork unitOfWork)
        {
            _uow = unitOfWork;
        }

        protected void Save()
        {
            _uow.Save();
        }

        public void Dispose()
        {
            _uow.Dispose();
        }
    }
}
