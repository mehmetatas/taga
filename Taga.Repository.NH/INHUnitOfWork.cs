using NHibernate;

namespace Taga.Repository.NH
{
    internal interface INHUnitOfWork
    {
        ISession Session { get; }
    }
}