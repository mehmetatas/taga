
namespace Taga.Core.Configuration
{
    public interface IConfigManager
    {
        IConfig GetConfig(string key);
    }
}
