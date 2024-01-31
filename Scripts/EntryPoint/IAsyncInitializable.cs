using Cysharp.Threading.Tasks;

namespace EntryPoint
{
    public interface IAsyncInitializable
    {
        UniTask InitializeAsync();
    }
}