using System.Threading.Tasks;
using Microsoft.Owin;

namespace Taga.Framework.Hosting.Owin
{
    public interface IOwinHandler
    {
        Task Invoke(IOwinContext context);
    }
}