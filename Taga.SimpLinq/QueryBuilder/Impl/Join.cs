using System.Reflection;

namespace Taga.SimpLinq.QueryBuilder.Impl
{
    class Join : IJoin
    {
        public PropertyInfo LeftProperty { get; set; }
        public PropertyInfo RightProperty { get; set; }
    }
}