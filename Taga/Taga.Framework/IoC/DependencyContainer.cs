
namespace Taga.Framework.IoC
{
    public static class DependencyContainer
    {
        public static IDependencyContainer Current { get; private set; }

        public static void Init(IDependencyContainer container)
        {
            Current = container;
        }
    }
}
