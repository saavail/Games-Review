using Cysharp.Threading.Tasks;
using DependencyInjector;
using UnityEngine;

namespace EntryPoint
{
    public interface IResourceLoader : IService
    {
        UniTask<Object> LoadAsync(string path);
    }
}