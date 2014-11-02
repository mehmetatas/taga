
namespace Taga.Core.IoC
{
    public static class ServiceProvider
    {
        private static IServiceProvider _provider;

        public static IServiceProvider Provider
        {
            get
            {
                return _provider;
            }
        }

        public static void Init(IServiceProvider serviceProvider)
        {
            _provider = serviceProvider;
        }
    }
}