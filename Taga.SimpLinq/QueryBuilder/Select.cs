using Taga.SimpLinq.QueryBuilder.Impl;

namespace Taga.SimpLinq.QueryBuilder
{
    public static class Select
    {
        public static ISelectQueryBuilder From<T>() where T : class, new()
        {
            return new SelectQueryBuilder(typeof (T));
        }
    }
}
