using Taga.Orm.Meta;

namespace Taga.Orm.Sql.Command
{
    public class CommandParameter
    {
        public string Name { get; set; }
        public object Value { get; set; }
        public ParameterMeta ParameterMeta { get; set; }
    }
}