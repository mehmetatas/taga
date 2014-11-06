using System.Reflection;

namespace Taga.SimpLinq.QueryBuilder.Impl
{
    class OrderBy : IOrderBy
    {
        public PropertyInfo OrderProperty { get; set; }
        public bool Descending { get; set; }
    }
}