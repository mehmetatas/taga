using System.Threading.Tasks;

namespace Taga.Framework.Hosting
{
    public interface IHttpResponse
    {
        void SetHeader(string key, string value);
        Task WriteAsync(string json);
    }
}