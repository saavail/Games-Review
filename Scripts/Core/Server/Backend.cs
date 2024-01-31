using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Core.UserStuff;
using Cysharp.Threading.Tasks;
using DependencyInjector;
using EntryPoint;
using Enums;
using UISystem;
using UnityEngine;
using UnityEngine.Networking;

namespace Core.Server
{
    public class Backend : IBackendService, IAsyncInitializable
    {
        private const string DevBackend = "https://dev-service.tezro.com/services/";
        private const string ProdBackend = "https://games-service.tezro.com/services/";
        
        private const int Timeout = 4;
        
        private readonly IResourceLoader _resourceLoader;
        private readonly IBackendWaiter _backendWaiter;
        
        private string _url;

        private UniTask _activeTask;

        public event Action OnException;

        public bool IsPending => !_activeTask.Equals(default(UniTask)) && !_activeTask.Status.IsCompleted();

        public Backend(IResourceLoader resourceLoader, BackendWaiter backendWaiter)
        {
            _resourceLoader = resourceLoader;
            
            _backendWaiter = backendWaiter;
            _backendWaiter.ForceHide();
        }

        public async UniTask InitializeAsync()
        {
            var buildSettings = await _resourceLoader.LoadAsync("Data/BuildSettingsData") as BuildSettingsData;
            _url = buildSettings != null && buildSettings.IsDevelopment ? DevBackend : ProdBackend;
            Debug.Log($"Set url link for backend, url - {_url}");
        }

        public async UniTask<User> GetUser(string token)
        {
            return await SendRequest<User>("getUser", new Dictionary<string, string>()
            {
                {nameof(token), token}
            }, canShowWaiter: false);
        }

        public async UniTask<User> BuySlot(string token, int slotId)
        {
            return await SendRequest<User>("buySlot", new Dictionary<string, string>()
            {
                {nameof(token), token},
                {nameof(slotId), slotId.ToString()},
            });
        }

        public async UniTask<RemoteBalance> GetBalance()
        {
            return await SendRequest<RemoteBalance>("getBalance", null, canShowWaiter: false, isOutOfTurn: true);
        }

        public async UniTask<User> ChangeNickname(string token, string newNickname)
        {
            return await SendRequest<User>("updateUsername", new Dictionary<string, string>()
            {
                {nameof(token), token},
                {"username", newNickname},
            });
        }

        public async UniTask<User> FinishGame(string token, GameType gameType, int score, int coins)
        {
            return await SendRequest<User>("updateGameProgress", new Dictionary<string, string>()
            {
                {nameof(token), token},
                {nameof(gameType), ((int)gameType).ToString()},
                {"experience", score.ToString()},
                {nameof(coins), coins.ToString()},
            }, canShowWaiter: false);
        }

        public async UniTask<bool> Revive(string token, GameType gameType)
        {
            string request = await SendRequest<string>("revive", new Dictionary<string, string>()
            {
                {nameof(token), token},
                {nameof(gameType), ((int)gameType).ToString()},
            }, false);
            
            return bool.TryParse(request, out bool result) && result;
        }

        public async UniTask<User> ChooseSkin(string token, int skinId)
        {
            return await SendRequest<User>("updateGameProgress", new Dictionary<string, string>()
            {
                {nameof(token), token},
                {nameof(skinId), skinId.ToString()},
            });
        }

        private async UniTask<T> SendRequest<T>(string postMethod, Dictionary<string, string> data, 
            bool castFromJson = true, bool canShowWaiter = true, bool isOutOfTurn = false) 
            where T : class
        {
            if (IsPending && !isOutOfTurn)
            {
                return await UniTask.FromResult(default(T));
            }

            string sendingUrl;
            
            if (data != default)
            {
                string parameters = string.Concat(data.Select(i => $"{i.Key}={i.Value}&"));
                sendingUrl = string.Concat($"{_url}{postMethod}?", parameters);
                sendingUrl = sendingUrl.Remove(sendingUrl.Length - 1, 1);
            }
            else
            {
                sendingUrl = $"{_url}{postMethod}";
            }

            var request = UnityWebRequest.Get(sendingUrl);
            int attempts = 2;
            
            if (canShowWaiter)
            {
                _backendWaiter.Show();
            }
            
            try
            {
                CancellationTokenSource tokenSource = new CancellationTokenSource();
                tokenSource.CancelAfterSlim(TimeSpan.FromSeconds(Timeout));

                do
                {
                    var task = request.SendWebRequest().ToUniTask(cancellationToken: tokenSource.Token);
                    _activeTask = task;
                    
                    await task;
                    
                    if (request.result is UnityWebRequest.Result.Success || !string.IsNullOrEmpty(request.downloadHandler?.text))
                        break;
                    
                } while (--attempts > 0);
                
                AllServices.Container.Single<IDebugConsole>().Post(request.downloadHandler?.text);
                _backendWaiter.Hide();

                if (request.downloadHandler == null || string.IsNullOrEmpty(request.downloadHandler.text))
                {
                    throw new UnityWebRequestException(default);
                }
                
                return castFromJson 
                    ? JsonUtility.FromJson<T>(request.downloadHandler?.text) 
                    : request.downloadHandler?.text as T;
            }
            catch (Exception e)
            {
                OnException?.Invoke();
                Debug.LogException(e);
            }

            _backendWaiter.Hide();
            return await UniTask.FromResult<T>(default);
        }
    }
}