namespace Taga.Framework.Hosting
{
    public interface IHttpResponse
    {
        void SetHeader(string key, string value);
        void SetContent(string json);
    }
}