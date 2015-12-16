using Taga.Orm.Db.Builders;
using Taga.Orm.Db.Builders.Impl;
using Taga.Orm.Providers;

namespace Taga.Orm.Db
{
    public static class Db
    {
        private static IDbProvider _defaultProvider;

        public static IDbBuilder Setup(IDbProvider provider)
        {
            if (_defaultProvider == null)
            {
                _defaultProvider = provider;
            }

            return new DbBuilder(provider);
        }
    }
}
