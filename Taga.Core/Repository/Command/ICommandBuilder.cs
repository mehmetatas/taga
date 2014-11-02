using System.Collections.Generic;

namespace Taga.Core.Repository.Command
{
    public interface ICommandBuilder
    {
        ICommand BuildCommand(string spNameOrSql, IDictionary<string, object> args = null, bool rawSql = false);
    }
}
