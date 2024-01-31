using System;
using System.Collections.Generic;
using System.Linq;
using Enums;
using JsonFx.Json;
using UnityEngine;

namespace Core.UserStuff
{
    [JsonOptIn]
    public class User : IUser
    {
        public const string GuestName = "Guest";

        public string id;
        public string username;
        public string avatarUrl;
        public int level;
        public int levelProgress;
        public List<Skin> skins;
        public List<Currency> currencies;
        public List<GameScore> gamesScore;

        public string Token => id;
        public bool IsGuest => string.IsNullOrEmpty(id) || id == GuestName;
        public float LevelProgress => levelProgress;
        public int Level => level;

        private User()
        {
            id = GuestName;
            username = GuestName;
            level = 1;
        }

        public User(List<Currency> money) : this()
        {
            currencies = money;
            skins = new();
            gamesScore = new();
        }

        public void AddMoneyDelta(CurrencyType type, float delta)
        {
            var money = currencies.FirstOrDefault(i => i.currencyType == type);
            
            if (money == default)
            {
                if (type != CurrencyType.None)
                {
                    currencies.Add(new Currency
                    {
                        currencyType = type,
                        amount = Mathf.Max(delta, 0)
                    });
                }
                return;
            }
            
            money.amount = Mathf.Round(money.amount + delta);
        }

        public void ChooseSkin(GameType gameType, SkinPart skinType, SkinType skinCollectionType)
        {
            Skin skin = skins.FirstOrDefault(i =>
                i.skinType == skinType && i.skinCollectionType == skinCollectionType && i.gameType == gameType);

            if (skin != default)
            {
                skin.isSelected = true;
            }
        }

        public void AddSkin(GameType gameType, SkinPart skinType, SkinType skinCollectionType, string name, int id)
        {
            if (skins.Any(i => i.skinId == id))
                return;
            
            skins.Add(new Skin()
            {
                gameType = gameType,
                skinId = id,
                skinName = name,
                isSelected = false,
                skinCollectionType = skinCollectionType,
                skinType = skinType
            });
        }

        public float GetMoney(CurrencyType type) 
            => currencies.FirstOrDefault(i => i.currencyType == type)?.amount ?? 0f;

        public bool CanBuy(CurrencyType type, float value)
            => GetMoney(type) >= value;

        public bool IsPurchased(int orderId)
            => skins != null && skins.Any(i => i.skinId == orderId);

        public SkinType GetChooseSkin(GameType gameType, SkinPart skinPart)
        {
            Func<Skin,bool> checkAction = i => i.gameType == gameType && i.skinType == skinPart;
            return skins.Any(checkAction) ? skins.First(checkAction).skinCollectionType : SkinType.Default;
        }

        public bool IsSkinChoose(int skinId) 
            => skins.FirstOrDefault(i => i.skinId == skinId)?.isSelected ?? false;

        public bool HasSkin(int skinId) 
            => skins.Any(i => i.skinId == skinId);
    }
}