using Enums;

namespace Core.UserStuff
{
    public interface IUser
    {
        public const string SaveKey = nameof(User);
        
        public string Token { get; }
        public bool IsGuest { get; }
        public float LevelProgress { get; }
        public int Level { get; }

        public float GetMoney(CurrencyType type);
        public bool CanBuy(CurrencyType type, float value);
        public bool IsPurchased(int orderId);
        public SkinType GetChooseSkin(GameType gameType, SkinPart skinPart);
        public bool IsSkinChoose(int id);
        public bool HasSkin(int id);
    }
}