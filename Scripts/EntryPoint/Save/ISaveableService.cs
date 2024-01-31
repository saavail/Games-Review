using DependencyInjector;

namespace EntryPoint.Save
{
    public interface ISaveableService : IService
    {
        public void Save();
    }
}