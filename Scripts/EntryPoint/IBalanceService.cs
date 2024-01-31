using Core;
using Core.UserStuff;
using DependencyInjector;
using Enums;
using UISystem.Shop;

namespace EntryPoint
{
    public interface IBalanceService : IService
    {
        public const string SaveKey = nameof(MetaBalance); 

        bool IsGuestBalance { get; } 
        RemoteBalance RemoteBalance { get; }
        
        public UserLevel GetLevel(int level);
        public SlotData GetShopSlot(int id);
        public GameBalance GetGame(GameType type);
    }
}