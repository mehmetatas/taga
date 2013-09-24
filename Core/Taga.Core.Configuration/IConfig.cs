
namespace Taga.Core.Configuration
{
    public interface IConfig
    {
        T Get<T>(string key);
        void Load(IConfigSource source);
    }
}
