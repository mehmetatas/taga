using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Taga.Core.Repository.SimpLinq
{
    class SimpLinqSqlCommand : ISqlCommand
    {
        public SimpLinqSqlCommand(string sql, IDictionary<string, object> parameters)
        {
            CommandText = sql;
            Parameters = new ReadOnlyDictionary<string, object>(parameters);
        }

        public string CommandText { get; private set; }
        public IReadOnlyDictionary<string, object> Parameters { get; private set; }

        public SqlCommandType CommandType
        {
            get
            {
                return SqlCommandType.Text;
            }
        }
    }
}