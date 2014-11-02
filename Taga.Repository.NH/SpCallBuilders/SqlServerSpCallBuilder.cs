
namespace Taga.Repository.NH.SpCallBuilders
{
    public class SqlServerSpCallBuilder : BaseSpCallBuilder
    {
        protected override string Command
        {
            get { return "exec"; }
        }

        protected override bool UseParanthesisForEmptyArgs
        {
            get { return false; }
        }
    }
}
