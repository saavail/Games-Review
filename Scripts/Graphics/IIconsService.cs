using Core;
using DependencyInjector;
using Enums;
using UnityEngine;

namespace Graphics
{
    public interface IIconsService : IService
    {
        public Sprite GetMoneyMessageIcon { get; }
        public Sprite GetGameIcon { get; }
        public Sprite GetIcon(string name);
        public Sprite GetIcon(CurrencyType currency);
        public Sprite GetIcon(GameType game);
        public Sprite GetIcon(CryptoType crypto);
        public Sprite GetShopPreview(int slotId);
        public Sprite[] GetSkin(GameType gameType, SkinType skinType, SkinPart skinPart);
    }
}