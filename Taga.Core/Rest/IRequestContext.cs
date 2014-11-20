
namespace Taga.Core.Rest
{
    public interface IRequestContext
    {
        string GetHeader(string name);

        void SetHeader(string name, string value);

        bool RollbackOnError { get; }
    }
}
