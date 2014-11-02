namespace Taga.Core.Repository.Command
{
    public class CommandParameter : ICommandParameter
    {
        public CommandParameter(char paramIdentifier, string name, object value)
        {
            ParamIdentifier = paramIdentifier;
            Name = name;
            Value = value;
        }

        public char ParamIdentifier { get; private set; }
        public string Name { get; private set; }
        public object Value { get; private set; }
    }
}