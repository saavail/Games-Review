using Core.UserStuff;
using DependencyInjector;
using EntryPoint;
using UISystem;

namespace Core
{
    public class GamesService : IService
    {
        public GameFactory Factory { get; }

        public GamesService(IResourceLoader resourceLoader, ISceneLoaderService sceneLoaderService, WindowSystem windowSystem, IUserService userService)
        {
            Factory = new GameFactory(resourceLoader, sceneLoaderService, windowSystem, userService);
        }

        public void CloseCurrent()
        {
            Factory.Current?.ExitAsync();
            Factory.DisposeCurrent();
        }
    }
}