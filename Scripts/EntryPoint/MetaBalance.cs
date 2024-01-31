using System.Linq;
using Core;
using Core.Server;
using Core.UserStuff;
using Cysharp.Threading.Tasks;
using Enums;
using UnityEngine;

namespace EntryPoint
{
    public class MetaBalance : AsyncInitializableAndLoad<RemoteBalanceData>, IBalanceService
    {
        private const string RemoteBalancePath = "Data/RemoteBalanceData";
        
        private readonly IBackendService _backendService;

        public bool IsGuestBalance { get; private set; }
        public RemoteBalance RemoteBalance { get; private set; }

        public MetaBalance(IResourceLoader resourceLoader, IBackendService backendService) : base(resourceLoader)
        {
            _backendService = backendService;
        }
        
        public override async UniTask InitializeAsync()
        {
            // получаем ремоут баланс
            RemoteBalance = await _backendService.GetBalance();

            // если получили сохраняем его и выходим
            if (RemoteBalance != default)
            {
                Save();
                IsGuestBalance = false;
                return;
            }

            IsGuestBalance = true;

            // если не получили то пытаемся найти сейв 
            if (RemoteBalance == default)
            {
                var json = PlayerPrefs.GetString(IBalanceService.SaveKey, null);

                if (!string.IsNullOrEmpty(json))
                {
                    RemoteBalance = JsonUtility.FromJson<RemoteBalance>(json);
                }
            }
            
            // если и сейв не нашли то подгружаем локальный
            RemoteBalance ??= (await _resourceLoader.LoadAsync(RemoteBalancePath) as RemoteBalanceData)?.RemoteBalance;
        }

        private void Save()
        {
            string json = JsonUtility.ToJson(RemoteBalance, false);
            PlayerPrefs.SetString(IBalanceService.SaveKey, json);
        }
        
        public UserLevel GetLevel(int level)
        {
            return RemoteBalance.levels[Mathf.Clamp(level, 0, RemoteBalance.levels.Length - 1)];
        }

        public SlotData GetShopSlot(int id)
        {
            return RemoteBalance.slotsData.FirstOrDefault(i => i.slotId == id);
        }

        public GameBalance GetGame(GameType type)
        {
            return RemoteBalance.gamesData.FirstOrDefault(i => i.gameType == type);
        }
    }
}