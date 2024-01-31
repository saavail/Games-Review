using System;
using Core.UserStuff;
using Cysharp.Threading.Tasks;
using DependencyInjector;
using Enums;

namespace Core.Server
{
    public interface IBackendService : IService
    {
        public event Action OnException;
        public bool IsPending { get; }
        public UniTask<User> GetUser(string token);
        public UniTask<User> BuySlot(string token, int slotId);
        public UniTask<RemoteBalance> GetBalance();
        public UniTask<User> ChangeNickname(string token, string newNickname);
        public UniTask<User> FinishGame(string token, GameType gameType, int score, int coins);
        public UniTask<bool> Revive(string token, GameType gameType);
        public UniTask<User> ChooseSkin(string token, int skinId);
    }
}