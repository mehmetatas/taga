using NHibernate;

namespace Taga.Repository.NH
{
    internal interface INHUnitOfWork
    {
        IStatelessSession Session { get; }
    }
}