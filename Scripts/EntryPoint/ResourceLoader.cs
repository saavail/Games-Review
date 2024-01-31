using Cysharp.Threading.Tasks;
using UnityEngine;

namespace EntryPoint
{
    public class ResourceLoader : IResourceLoader
    {
        public UniTask<Object> LoadAsync(string path)
        {
            return Resources.LoadAsync(path).ToUniTask();
        }
    }
}