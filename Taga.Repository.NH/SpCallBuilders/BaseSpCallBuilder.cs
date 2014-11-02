using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Taga.Repository.NH.SpCallBuilders
{
    public abstract class BaseSpCallBuilder : INHSpCallBuilder
    {
        public virtual string BuildSpCall(string spNameOrSql, IDictionary<string, object> args)
        {
            var sb = new StringBuilder();

            sb.Append(Command)
                .Append(" ")
                .Append(spNameOrSql);

            var hasArg = args != null && args.Any();
            var useParanthesis = hasArg || UseParanthesisForEmptyArgs;

            if (useParanthesis)
            {
                sb.Append("(");
            }

            if (args != null)
            {
                sb.Append(String.Join(",", args.Keys.Select(p => String.Format(":{0}", p))));
            }

            if (useParanthesis)
            {
                sb.Append(")");
            }

            return sb.ToString();
        }

        protected abstract string Command { get; }

        protected abstract bool UseParanthesisForEmptyArgs { get; }
    }
}
