using Cysharp.Threading.Tasks;
using DependencyInjector;
using UnityEngine.SceneManagement;

namespace EntryPoint
{
    public interface ISceneLoaderService : IService
    {
        UniTask LoadSceneAsync(string sceneName, LoadSceneMode loadSceneMode = LoadSceneMode.Single);
        UniTask UnLoadSceneAsync(string sceneName);
    }
}