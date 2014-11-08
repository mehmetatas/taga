using System.Collections.Generic;

namespace Taga.Core.Repository
{
    public interface ISqlCommand
    {
        string CommandText { get; }
        IReadOnlyDictionary<string, object> Parameters { get; }
        SqlCommandType CommandType { get; }
    }
}
