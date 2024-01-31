using System;
using Core.Server;
using Core.UserStuff;
using Cysharp.Threading.Tasks;
using DependencyInjector;
using EntryPoint;
using Enums;
using Graphics;
using Hub;
using UISystem;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core
{
    public abstract class MiniGame : AsyncInitializableAndLoad<GameData>
    {
        private readonly ISceneLoaderService _sceneLoaderService;
        private readonly WindowSystem _windowSystem;
        private readonly IUserService _userService;
        
        protected GameReferencesHolder _referencesHolder;

        private string Name => GetType().Name;
        private string ScoreSaveKey => Name + nameof(Score);
        private string MaxScoreSaveKey => Name + nameof(MaxScoreSaveKey);
        private string TutorialShowSaveKey => Name + nameof(TutorialShowSaveKey);
        protected override string LoadPath => Name + "Data";

        public abstract GameType GameType { get; }
        public int Score { get; protected set; }
        public bool UseRevive { get; protected set; }

        protected MiniGame(IResourceLoader resourceLoader, ISceneLoaderService sceneLoaderService, 
            WindowSystem windowSystem, IUserService userService) 
            : base(resourceLoader)
        {
            _sceneLoaderService = sceneLoaderService;
            _windowSystem = windowSystem;
            _userService = userService;
        }

        public override async UniTask InitializeAsync()
        {
            UniTask initializeAsync = base.InitializeAsync();
            UniTask sceneAsync = _sceneLoaderService.LoadSceneAsync(Name);

            await UniTask.WhenAll(initializeAsync, sceneAsync);

            foreach (var go in SceneManager.GetActiveScene().GetRootGameObjects())
            {
                if (go.TryGetComponent<GameReferencesHolder>(out var referencesHolder))
                {
                    _referencesHolder = referencesHolder;
                    _referencesHolder.Initialize();
                    break;
                }
            }
            
            _windowSystem.CloseAll(() => _windowSystem.UserProfile.CloseAll());

            foreach (var startable in _referencesHolder.Startables) 
                startable.OnStart(Data);
            
            _windowSystem.Show<ScoreWindow>().SetMaxScore(GetMaxScore());
            
            if (IsTutorialShow())
            {
                OnStart();
            }
            else
            {
                ShowTutorial();
            }
        }
        
        protected abstract void ShowTutorial();

        public virtual void OnTutorialShowed()
        {
            PlayerPrefs.SetInt(TutorialShowSaveKey, 1);
        }

        private void OnReviveClick()
        {
            Revival().Forget();
        }

        private void UnsubscribeReviveWindow()
        {
            var window = _windowSystem.GetWindow<RevivalWindow>();
            window.OnReviveClick -= OnReviveClick;
            window.OnTimeEnded -= OnTimeEnded;
        }

        private void OnTimeEnded()
        {
            if (AllServices.Container.Single<IBackendService>().IsPending)
                return;
            
            _windowSystem.CloseCurrentWindow();
            UnsubscribeReviveWindow();
        }

        protected abstract void OnStart();

        protected void Die()
        {
            _windowSystem.GetWindow<ScoreWindow>().SetActiveRestartButton(false);
            
            var user = AllServices.Container.Single<IUserService>().User;
            var revivalType = Data.ReviveCosts[0].CurrencyType;
            var revivalCost = Data.ReviveCosts[0].Value;
            
            if (user.GetMoney(revivalType) >= revivalCost && !UseRevive)
            {
                Sprite icon = AllServices.Container.Single<IIconsService>().GetIcon(revivalType);
                
                var window = _windowSystem.Show<RevivalWindow>();
                window.SetRevivalCost(revivalCost, icon);
                
                window.OnReviveClick += OnReviveClick;
                window.OnTimeEnded += OnTimeEnded;
            }
            else
            {
                _windowSystem.Show<RewardWindow>().SetWindowValues(Score, GetMaxScore(), CalculateCoins());
            }

            Save();
            
            foreach (var deadable in _referencesHolder.Deadables) 
                deadable.OnDied();
            
            OnDied();
        }

        private async UniTaskVoid Revival()
        {
            if (AllServices.Container.Single<IBackendService>().IsPending)
                return;
            
            await _userService.UseRevive(this, isSuccess =>
            {
                if (isSuccess)
                {
                    foreach (var restartable in _referencesHolder.Restartables) 
                        restartable.OnRestart(true);
            
                    _windowSystem.GetWindow<ScoreWindow>().SetActiveRestartButton(true);
                    UseRevive = true;

                    _windowSystem.CloseCurrentWindow();

                    OnRevival();
                }
                else
                {
                    _windowSystem.CloseCurrentWindow();
                }
            });
            
            UnsubscribeReviveWindow();
        }
        
        protected abstract void OnRevival();
        
        public virtual int CalculateCoins()
        {
            return (int)Math.Round(Data.CoinsByScore * Score, MidpointRounding.AwayFromZero);
        }

        protected abstract void OnDied();

        public void Restart()
        {
            CoreRestart();
            
            var scoreWindow = _windowSystem.GetWindow<ScoreWindow>();
            scoreWindow.SetActiveRestartButton(true);
            UseRevive = false;
            
            _windowSystem.CloseCurrentWindow();

            OnRestart(false);
        }

        protected abstract void OnRestart(bool isFast);

        public void FastRestart()
        {
            CoreRestart();
            OnRestart(true);
        }

        private void CoreRestart()
        {
            foreach (var restartable in _referencesHolder.Restartables)
                restartable.OnRestart(false);

            var scoreWindow = _windowSystem.GetWindow<ScoreWindow>();
            scoreWindow.ResetValues();
            scoreWindow.SetMaxScore(GetMaxScore());
            ApplyRewards();
        }

        private void ApplyRewards()
        {
            _userService.FinishGame(this);
        }

        protected void SetScore(int score)
        {
            if (score <= Score)
                return;

            Score = score;
            
            int maxScore = GetMaxScore();

            if (score > maxScore)
            {
                maxScore = score;
                PlayerPrefs.SetInt(MaxScoreSaveKey, score);
            }
            
            foreach (var scoreChangeable in _referencesHolder.ScoreChangeables) 
                scoreChangeable.OnScoreChanged(score, maxScore);
            
            UpdateScoreCoinValues();
            OnScoreChanged(score, maxScore);
        }

        protected void UpdateScoreCoinValues()
        {
            _windowSystem.GetWindow<ScoreWindow>().SetWindowValues(Score, CalculateCoins(), GetMaxScore());
        }

        protected abstract void OnScoreChanged(int score, int maxScore);

        public int GetMaxScore()
            => PlayerPrefs.GetInt(MaxScoreSaveKey, 0);

        public bool IsTutorialShow() => PlayerPrefs.HasKey(TutorialShowSaveKey);

        public void Save()
        {
            if (Score > PlayerPrefs.GetInt(MaxScoreSaveKey, 0))
                PlayerPrefs.SetInt(MaxScoreSaveKey, Score);
            
            PlayerPrefs.SetInt(ScoreSaveKey, Score);

            foreach (var saveable in _referencesHolder.Saveables) 
                saveable.OnSave(Name);

            OnSave();
            PlayerPrefs.Save();
        }
        
        protected abstract void OnSave();

        public async void ExitAsync()
        {
            Save();
            ApplyRewards();

            await _sceneLoaderService.LoadSceneAsync(EntryPoint.EntryPoint.MainSceneName);
            
            _windowSystem.CloseAll();
            _windowSystem.Show<HubWindow>(() => _windowSystem.UserProfile.OpenAll());

            foreach (var exitable in _referencesHolder.Exitables) 
                exitable.OnExit();

            AllServices.Container.Single<WindowSystem>().UserProfile.UpdateWalletInfo();
            OnExit();

            var revivalWindow = _windowSystem.GetWindow<RevivalWindow>();
            
            if (revivalWindow != default)
            {
                revivalWindow.OnReviveClick -= OnReviveClick;
            }
        }

        protected abstract void OnExit();
    }
}