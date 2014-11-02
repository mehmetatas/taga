
namespace Taga.Repository.NH.SpCallBuilders
{
    public class OracleSpCallBuilder : BaseSpCallBuilder
    {
        protected override string Command
        {
            get { return "call"; }
        }

        protected override bool UseParanthesisForEmptyArgs
        {
            get { return true; }
        }
    }
}
