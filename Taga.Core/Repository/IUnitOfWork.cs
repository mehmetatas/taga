using System;

namespace Taga.Core.Repository
{
    public interface IUnitOfWork : IDisposable
    {
        void Save();
    }
}