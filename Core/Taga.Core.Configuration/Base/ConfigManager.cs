
namespace Taga.Core.Configuration.Base
{
    public class ConfigManager
    {
        #region Singleton

        private static readonly SetOnce<IConfigManager> Manager = new SetOnce<IConfigManager>("Config Manager");

        public static IConfigManager Instance
        {
            get { return Manager.Value; }
            set { Manager.Value = value; }
        }

        #endregion
    }
}
