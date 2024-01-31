using System;
using Cysharp.Threading.Tasks;
using EntryPoint.Save;
using Enums;

namespace Core.UserStuff
{
    public interface IUserService : ISaveableService
    {
        public bool IsOnline { get; }
        public IUser User { get; }
        public UniTask BuySlot(SlotData data, Action callback = null);
        public UniTask ChooseSkin(SlotData slotData, Action callback = null);
        public UniTask FinishGame(MiniGame game, Action callback = null);
        public UniTask<bool> UseRevive(MiniGame game, Action<bool> callback = null);
        public UniTask ChangeNickname(string newNickname, Action callback = null);
        public UserLevel GetCurrentLevel();
        public float GetLevelPercent();
    }
}