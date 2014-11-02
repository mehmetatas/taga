using System.Reflection;

namespace Taga.Core.Repository.Mapping
{
    public class ColumnMapping
    {
        public PropertyInfo PropertyInfo { get; set; }
        public string ColumnName { get; set; }
        public bool IsId { get; set; }
        public bool IsAutoIncrement { get; set; }
    }
}