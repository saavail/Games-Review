using System.Threading.Tasks;
using Core.UserStuff;
using Cysharp.Threading.Tasks;
using DependencyInjector;
using EntryPoint;
using FlappyPlane.Scripts;
using Games;
using Games.RoosterGame.Core;
using Games.Stack.Core;
using UISystem;

namespace Core
{
    public class GameFactory : IService
    {
        private readonly IResourceLoader _resourceLoader;
        private readonly ISceneLoaderService _sceneLoaderService;
        private readonly WindowSystem _windowSystem;
        private readonly IUserService _userService;

        public MiniGame Current { get; private set; }

        public GameFactory(IResourceLoader resourceLoader, ISceneLoaderService sceneLoaderService, WindowSystem windowSystem, IUserService userService)
        {
            _resourceLoader = resourceLoader;
            _sceneLoaderService = sceneLoaderService;
            _windowSystem = windowSystem;
            _userService = userService;
        }
        
        public async UniTask<Game2048> Create2048()
            => await Initialize(new Game2048(_resourceLoader, _sceneLoaderService, _windowSystem, _userService));

        public async UniTask<GameFlappyPlane> CreateFlappyPlane()
            => await Initialize(new GameFlappyPlane(_resourceLoader, _sceneLoaderService, _windowSystem, _userService));

        public async UniTask<RoosterRunner> CreateRoosterRunner()
            => await Initialize(new RoosterRunner(_resourceLoader, _sceneLoaderService, _windowSystem, _userService));
        
        public async UniTask<GameStack> CreateStack()
            => await Initialize(new GameStack(_resourceLoader, _sceneLoaderService, _windowSystem, _userService));

        private async Task<TGame> Initialize<TGame>(TGame game)
            where TGame : MiniGame
        {
            await game.InitializeAsync();
            Current = game;
            return game;
        }

        public void DisposeCurrent()
        {
            Current = null;
        }
    }
}