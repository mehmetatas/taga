using System;
using Taga.UserApp.Core.Repository;

namespace Taga.UserApp.Core.Database
{
    public interface IReadonlyDb : IRepositoryProvider, IDisposable
    {
    }
}