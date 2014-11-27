
namespace Taga.Core.Rest
{
    public interface IRequestContext
    {
        string GetRequestHeader(string name);

        void SetResponseHeader(string name, string value);
    }
}
