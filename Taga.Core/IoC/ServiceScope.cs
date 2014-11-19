namespace Taga.Core.IoC
{
    public enum ServiceScope
    {
        Transient,
        PerThread,
        PerWebRequest,
        Singleton
    }
}