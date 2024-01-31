using System;
using System.Linq;
using System.Threading.Tasks;
using Core.Server;
using Cysharp.Threading.Tasks;
using DependencyInjector;
using EntryPoint;
using EntryPoint.Save;
using UISystem;
using UISystem.Shop;
using UnityEngine;

namespace Core.UserStuff
{
    public class UserService : AsyncInitializableAndLoad<UserData>, IUserService
    {
        private readonly IBalanceService _balanceService;
        private readonly WindowSystem _windowSystem;
        private IBackendService _backendService;
        private User _user;

        private Func<UniTask> _lastRequest;

        public bool IsOnline { get; private set; }
        public IUser User => _user;

        public UserService(IResourceLoader resourceLoader, IBackendService backendService, IBalanceService balanceService, WindowSystem windowSystem)
            : base(resourceLoader)
        {
            _backendService = backendService;
            _balanceService = balanceService;
            _windowSystem = windowSystem;

            ShopSlotBase.OnSlotClick += ShopSlotBase_OnSlotClick;
            backendService.OnException += BackendService_OnException;
        }

        private void BackendService_OnException()
        {
            _windowSystem.Show<NoInternetPopup>()
                .SetupActions(() =>
                {
                    TryAgainLastRequest();
                    ConvertToGuest();
                }, ConvertToGuest);
        }

        private void TryAgainLastRequest()
        {
            _lastRequest?.Invoke();
        }

        private void ShopSlotBase_OnSlotClick(SlotData data, Action callback)
        {
            if (User.IsPurchased(data.slotId))
            {
                if (User.IsSkinChoose(data.slotId))
                {
                    callback?.Invoke();
                }
                else
                {
                    ChooseSkin(data, callback).Forget();
                }
            }
            else
            {
                BuySlot(data, callback).Forget();
            }
        }

        public override async UniTask InitializeAsync()
        {
            async Task DownloadUser(string token)
            {
                User remoteUser = await _backendService.GetUser(token);

                if (remoteUser != default)
                {
                    _user = remoteUser;
                    IsOnline = true;
                }
            }
            
            var json = PlayerPrefs.GetString(IUser.SaveKey, null);

            // если есть сохранение 
            if (!string.IsNullOrEmpty(json))
            {
                _user = JsonUtility.FromJson<User>(json);
                IsOnline = false;

                // если не гость то пытаемся получить его через бэк, если не получаем оставлям сейв
                if (!User.IsGuest)
                {
                    await DownloadUser(_user.Token);
                }
                // если не гость то пробуем инит, если нет токена то оставляем сейв
                else
                {
                    string token = GetToken();
                    
                    if (!string.IsNullOrEmpty(token))
                    {
                        await DownloadUser(token);
                    }
                }
            }
            // если нет сохранения
            else 
            {
                // подгружаем статы начального гостя
                await base.InitializeAsync();

                string token = GetToken();

                // если есть токен то пытаемся пулить его через бэк, если не получаем то создаем гостя
                if (!string.IsNullOrEmpty(token))
                {
                    User user = await _backendService.GetUser(token);

                    if (user != default)
                    {
                        _user = user;
                        IsOnline = true;
                    }
                    else
                    {
                        _user = new User(Data.Money);
                    }
                }
                // если нет сохранения и токена то создаем гостя
                else 
                {
                    _user = new User(Data.Money);
                }
            }

            if (!IsOnline)
            {
                ConvertToGuest();
            }
        }

        private void ConvertToGuest()
        {
            FakeBackend fakeBackend = new FakeBackend(_resourceLoader, _user, _balanceService);
            AllServices.Container.RegisterSingle<IBackendService>(fakeBackend);
            _backendService = fakeBackend;
        }

        private string GetToken()
        {
#if UNITY_EDITOR
            return string.Empty;
#endif
            if (string.IsNullOrEmpty(Application.absoluteURL))
                return string.Empty;
            
            AllServices.Container.Single<IDebugConsole>().Post($"{Application.absoluteURL}");

            string[] urls = Application.absoluteURL.Split('=');

            if (urls == null || urls.Length < 2)
                return string.Empty;

            return urls.Last();
        }

        public async UniTask ChooseSkin(SlotData slotData, Action callback = null)
        {
            await UserRequest(_backendService.ChooseSkin(User.Token, slotData.slotId), callback);
        }

        public async UniTask FinishGame(MiniGame game, Action callback = null)
        {
            await UserRequest(_backendService.FinishGame(User.Token, game.GameType, game.Score, game.CalculateCoins()),
                callback);
        }

        public async UniTask<bool> UseRevive(MiniGame game, Action<bool> callback = null)
        {
            _lastRequest = () => UseRevive(game, callback);
            
            bool isSuccess = await _backendService.Revive(User.Token, game.GameType);
            callback?.Invoke(isSuccess);
            return isSuccess;
        }

        public async UniTask ChangeNickname(string newNickname, Action callback = null)
        {
            await UserRequest(_backendService.ChangeNickname(User.Token, newNickname), callback);
        }

        public UserLevel GetCurrentLevel()
        {
            return _balanceService.GetLevel(_user.Level);
        }

        public float GetLevelPercent()
        {
            return _user.LevelProgress / GetCurrentLevel().requiredExperience;
        }

        public async UniTask BuySlot(SlotData data, Action callback = null)
        {
            await UserRequest(_backendService.BuySlot(User.Token, data.slotId), callback);
        }

        private async UniTask UserRequest(UniTask<User> request, Action callback = null)
        {
            _lastRequest = () => UserRequest(request, callback);
            
            User newUser = await request;

            if (newUser != default)
            {
                _user = newUser;
            }
            
            callback?.Invoke();
        }

        void ISaveableService.Save()
        {
#if UNITY_EDITOR
            string json = JsonUtility.ToJson(_user, true);
            Debug.Log(json);
#else
            string json = JsonUtility.ToJson(_user, false);
#endif
            PlayerPrefs.SetString(IUser.SaveKey, json);
        }
    }
}