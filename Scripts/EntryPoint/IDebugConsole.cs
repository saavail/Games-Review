using DependencyInjector;

namespace EntryPoint
{
    public interface IDebugConsole : IService
    {
        public void Initialize();
        public void Post(string message);
    }
}