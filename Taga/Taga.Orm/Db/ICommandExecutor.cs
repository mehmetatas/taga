using System.Data;
using Taga.Orm.Sql.Command;

namespace Taga.Orm.Db
{
    public interface ICommandExecutor
    {
        int ExecuteNonQuery(Command cmd);
        object ExecuteScalar(Command cmd);
        IDataReader ExecuteReader(Command cmd);
    }
}