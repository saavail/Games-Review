using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EntryPoint
{
    public class SceneLoader : ISceneLoaderService
    {
        private readonly IGameLoggerService _loggerService;

        public SceneLoader(IGameLoggerService loggerService)
        {
            _loggerService = loggerService;
        }
        
        public async UniTask LoadSceneAsync(string sceneName, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            _loggerService.Log(LogType.Develop, $"started load scene {sceneName} at {Time.realtimeSinceStartup}");
            await SceneManager.LoadSceneAsync(sceneName, loadSceneMode);
            _loggerService.Log(LogType.Develop, $"{sceneName} scene loaded at {Time.realtimeSinceStartup}");
        }
        
        public async UniTask UnLoadSceneAsync(string sceneName)
        {
            _loggerService.Log(LogType.Develop, $"started unload scene {sceneName} at {Time.realtimeSinceStartup}");
            await SceneManager.UnloadSceneAsync(sceneName, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
            _loggerService.Log(LogType.Develop, $"{sceneName} scene unloaded at {Time.realtimeSinceStartup}");
        }
    }
}