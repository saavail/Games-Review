using System;
using System.Linq;
using Core.Server.FakeData;
using Core.UserStuff;
using Cysharp.Threading.Tasks;
using EntryPoint;
using Enums;
using UnityEngine;

namespace Core.Server
{
    public class FakeBackend : AsyncInitializableAndLoad<FakeBackendData>, IBackendService
    {
        private readonly User _user;
        private readonly IBalanceService _balanceService;

        public event Action OnException;
        public bool IsPending => false;

        public FakeBackend(IResourceLoader resourceLoader, User user, IBalanceService balanceService)
            : base(resourceLoader)
        {
            _user = user;
            _balanceService = balanceService;
        }

        public UniTask<User> GetUser(string token)
        {
            return default;
        }

        public UniTask<User> BuySlot(string token, int slotId)
        {
            SlotData slot = _balanceService.GetShopSlot(slotId);

            if (slot.IsDefault || _user.HasSkin(slotId))
                return default;
            
            var priceType = slot.price.currencyType;
            float price = slot.price.amount;

            if (!_user.CanBuy(priceType, price))
                return default;

            _user.AddMoneyDelta(priceType, -price);
            _user.AddMoneyDelta(slot.reward.currencyType, slot.reward.amount);

            if (slot.IsSkin)
            {
                _user.AddSkin(slot.gameType, slot.skinType, slot.skinCollectionType, slot.skinName, slotId);
            }
            
            return default;
        }

        public UniTask<RemoteBalance> GetBalance()
        {
            return default;
        }

        public UniTask<User> ChangeNickname(string token, string newNickname)
        {
            _user.username = newNickname;
            return default;
        }

        public UniTask<User> FinishGame(string token, GameType gameType, int score, int coins)
        {
            GameBalance balance = _balanceService.GetGame(gameType);

            if (Equals(balance, default))
                return default;
            
            _user.AddMoneyDelta(CurrencyType.Gold, coins);
            AddLevelProgress(Mathf.RoundToInt(score * balance.experienceMultiplier));

            GameScore gameScore = _user.gamesScore.FirstOrDefault(i => i.gameType == gameType);
            
            if (gameScore != default)
            {
                gameScore.progress = Mathf.Max(gameScore.progress, score);
            }
            else
            {
                _user.gamesScore.Add(new GameScore()
                {
                    gameType = gameType,
                    progress = Mathf.Max(0, score)
                });
            }

            return default;
        }

        private void AddLevelProgress(int exp)
        {
            var levels = _balanceService.RemoteBalance.levels;
            int currentLevelIndex = _user.Level;
            
            _user.levelProgress += exp;

            while (_user.levelProgress >= levels[currentLevelIndex].requiredExperience && currentLevelIndex < levels.Length)
            {
                _user.level++;
                _user.levelProgress -= levels[currentLevelIndex].requiredExperience;
                
                _user.AddMoneyDelta(levels[currentLevelIndex].reward.currencyType, levels[currentLevelIndex].reward.amount);
                currentLevelIndex++;
            }
        }

        public UniTask<bool> Revive(string token, GameType gameType)
        {
            GameBalance balance = _balanceService.GetGame(gameType);

            var reviveType = balance.reviveCost.currencyType;
            var revivePrice = balance.reviveCost.amount;
            
            if (Equals(balance, default) || !_user.CanBuy(reviveType, revivePrice))
                return default;
            
            _user.AddMoneyDelta(reviveType, -revivePrice);
            return UniTask.FromResult(true);
        }

        public UniTask<User> ChooseSkin(string token, int skinId)
        {
            SlotData slot = _balanceService.GetShopSlot(skinId);

            if (slot.IsDefault || !slot.IsSkin)
                return default;

            _user.ChooseSkin(slot.gameType, slot.skinType, slot.skinCollectionType);
            return default;
        }
    }
}