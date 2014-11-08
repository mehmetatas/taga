using System;
using System.Linq;
using System.Text;
using Taga.Core.Repository.Command;

namespace Taga.Repository.NH.SpCallBuilders
{
    public abstract class BaseSpCallBuilder : INHSpCallBuilder
    {
        public virtual string BuildSpCall(ICommand cmd)
        {
            var sb = new StringBuilder();

            sb.Append(Command)
                .Append(" ")
                .Append(cmd.CommandText);

            var parameters = cmd.Parameters;

            var hasArg = parameters != null && parameters.Any();
            var forceParanthesis = hasArg || ForceParanthesisForEmptyArgs;

            if (forceParanthesis)
            {
                sb.Append("(");
            }

            if (parameters != null)
            {
                sb.Append(String.Join(",", parameters.Select(p => String.Format("{0}{1}", p.ParamIdentifier, p.Name))));
            }

            if (forceParanthesis)
            {
                sb.Append(")");
            }

            return sb.ToString();
        }

        protected abstract string Command { get; }

        protected abstract bool ForceParanthesisForEmptyArgs { get; }
    }
}
