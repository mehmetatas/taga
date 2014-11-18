using System.Data;
using Taga.Core.Repository.Command;

namespace Taga.Repository.Hybrid.Commands
{
    public class NonQueryCommand : IHybridUowCommand
    {
        private readonly ICommand _command;

        public NonQueryCommand(ICommand command)
        {
            _command = command;
        }

        public void Execute(IDbCommand cmd)
        {
            cmd.CommandText = _command.CommandText;

            cmd.CommandType = _command.IsRawSql
                ? CommandType.Text
                : CommandType.StoredProcedure;

            if (_command.Parameters != null)
            {
                foreach (var parameter in _command.Parameters)
                {
                    var param = cmd.CreateParameter();

                    param.ParameterName = parameter.Name;

                    param.Value = parameter.Value;

                    cmd.Parameters.Add(param);
                }
            }

            cmd.ExecuteNonQuery();
        }
    }
}
