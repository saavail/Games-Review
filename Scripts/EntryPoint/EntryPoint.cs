using System.Collections.Generic;
using System.Threading.Tasks;
using Core;
using Core.Server;
using Core.UserStuff;
using Cysharp.Threading.Tasks;
using DependencyInjector;
using Graphics;
using Hub;
using UISystem;
using UnityEngine;
using Utilities.Pool;

namespace EntryPoint
{
    public class EntryPoint : MonoBehaviour
    {
        public const string MainSceneName = "Hub";
        
        [SerializeField]
        private SplashScreen _splashScreen;

        [SerializeField]
        private DebugConsole _debugConsole;

        [SerializeField]
        public BackendWaiter _backendWaiter;

        private async void Awake()
        {
            Input.multiTouchEnabled = false;
            Physics.autoSimulation = false;
            Application.targetFrameRate = 60;

            _splashScreen.Show();

            List<IAsyncInitializable> asyncInitializables = new();

            // AllServices.Container.RegisterSingle<IDebugConsole>(_debugConsole);
            AllServices.Container.RegisterSingle<IDebugConsole>(new EmptyConsole());
            _debugConsole.Initialize();

            var resourceLoader = AllServices.Container.RegisterSingle<IResourceLoader>(new ResourceLoader());

            var gameSettingsLoader = new GameSettingsLoader(resourceLoader);
            await gameSettingsLoader.InitializeAsync();

            var logger = AllServices.Container.RegisterSingle<IGameLoggerService>(new GameLogger(gameSettingsLoader.Data.LogType));
            var sceneLoader = AllServices.Container.RegisterSingle<ISceneLoaderService>(new SceneLoader(logger));

            RegisterInitializableService<Pool, IPoolService>(asyncInitializables, new Pool());
            RegisterInitializableService<IconManager, IIconsService>(asyncInitializables, new IconManager(resourceLoader));
            
            RegisterInitializableService<Game, IGameService>(asyncInitializables, new Game(resourceLoader));
            
            var windowSystem = new WindowSystem(resourceLoader);
            RegisterInitializableService<WindowSystem, WindowSystem>(asyncInitializables, windowSystem);

            Backend backend = new Backend(resourceLoader, _backendWaiter);
            RegisterInitializableService<Backend, IBackendService>(asyncInitializables, backend);

            // AllServices.Container.RegisterSingle<IBackendService>(backend);
            // FakeBackend backend = new FakeBackend(resourceLoader);
            // RegisterInitializableService<FakeBackend, IBackendService>(asyncInitializables, backend);
            
            await WaitInitializables(asyncInitializables);
            
            var localBalance = new MetaBalance(resourceLoader, backend);
            RegisterInitializableService<MetaBalance, IBalanceService>(asyncInitializables, localBalance);
            
            await WaitInitializables(asyncInitializables);

            var userService = new UserService(resourceLoader, backend, localBalance, windowSystem);
            RegisterInitializableService<UserService, IUserService>(asyncInitializables, userService);
            
            await WaitInitializables(asyncInitializables);
            logger.Log(LogType.Develop, $"Entry point initialized {Time.realtimeSinceStartup}");

            AllServices.Container.RegisterSingle(new GamesService(resourceLoader, sceneLoader, windowSystem, userService));

            await sceneLoader.LoadSceneAsync(MainSceneName);

            ShowStartMenu(windowSystem, userService);

            _splashScreen.Hide();
        }

        private void ShowStartMenu(WindowSystem windowSystem, IUserService userService)
        {
            windowSystem.Show<HubWindow>();
            windowSystem.UserProfile.Initialize(userService);
        }

        private static async Task WaitInitializables(List<IAsyncInitializable> asyncInitializables)
        {
            await UniTask.WhenAll(asyncInitializables.Select(i => i.InitializeAsync()));
            asyncInitializables.Clear();
        }

        private void RegisterInitializableService<TService, TRegisterService>(List<IAsyncInitializable> asyncInitializables, TService service)
            where TRegisterService : IService
            where TService : TRegisterService, IAsyncInitializable
        {
            asyncInitializables.Add(service);
            AllServices.Container.RegisterSingle<TRegisterService>(service);
        }
    }
}