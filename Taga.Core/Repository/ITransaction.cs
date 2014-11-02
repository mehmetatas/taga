using System;

namespace Taga.Core.Repository
{
    public interface ITransaction : IDisposable
    {
        void Commit();

        void Rollback();
    }
}
