using Taga.SimpLinq.QueryBuilder.Impl;

namespace Taga.SimpLinq.QueryBuilder
{
    public static class Select
    {
        public static ISelectQueryBuilder From<T>(IPropertyFilter propFilter = null) where T : class, new()
        {
            return new SelectQueryBuilder(typeof(T), propFilter ?? new DefaultPropertyFilter());
        }
    }
}
