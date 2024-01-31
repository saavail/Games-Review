using Core;
using Core.UserStuff;
using DependencyInjector;
using EntryPoint;
using Enums;
using UISystem;
using UISystem.Tutorial;
using UnityEngine;

namespace Games.RoosterGame.Core
{
    public class RoosterRunner : MiniGame
    {
        private new RoosterReferenceHolder _referencesHolder;
        private Rooster _rooster;

        public override GameType GameType => GameType.RoosterRunner;
        private new RoosterRunnerData Data { get; set; }

        public RoosterRunner(IResourceLoader resourceLoader, ISceneLoaderService sceneLoaderService,
            WindowSystem windowSystem, IUserService userService)
            : base(resourceLoader, sceneLoaderService, windowSystem, userService) { }

        private void PrepareRooster()
        {
            _rooster.transform.position = new Vector3(_referencesHolder.Map[1], 0, 0);
            _rooster.InitializePositionIndexes(1);
        }

        protected override void ShowTutorial()
        {
            OnStart();
            AllServices.Container.Single<WindowSystem>().Show<RoosterRunnerTutorial>();
        }

        public override void OnTutorialShowed()
        {
            _rooster.Initialize(_referencesHolder.Map, _referencesHolder.SpeedController, Data);
            base.OnTutorialShowed();
        }

        protected override void OnStart()
        {
            _referencesHolder = (RoosterReferenceHolder)base._referencesHolder;
            Data = (RoosterRunnerData)base.Data;

            _referencesHolder.SwipeReceiver.Swiped += SwipeReceiver_Swiped;
            _referencesHolder.ScoreController.OnScoreChanged += ScoreController_OnScoreChanged;
            _referencesHolder.CoinsController.OnCoinsChanged += CoinsController_OnCoinsChanged;

            _rooster = Object.Instantiate(Data.RoosterPrefab, new Vector3(_referencesHolder.Map[1], 0, 0),
                Quaternion.identity);
            
            _rooster.OnDie += Rooster_OnDie;

            if (IsTutorialShow())
            {
                _rooster.Initialize(_referencesHolder.Map, _referencesHolder.SpeedController, Data);
            }
            PrepareRooster();

            foreach (var runnerCreated in _referencesHolder.RunnerCreated) 
                runnerCreated.OnCreate(_rooster);
        }

        private void SwipeReceiver_Swiped(Swipe swipe)
        {
            if (_rooster == null || !_rooster.IsAlive)
                return;

            _rooster.HandleSwipe(swipe);
        }

        private void ScoreController_OnScoreChanged(int score)
        {
            SetScore(score);
        }

        private void CoinsController_OnCoinsChanged(int coins)
        {
            UpdateScoreCoinValues();
        }

        public override int CalculateCoins()
        {
            return _referencesHolder.CoinsController.Coins;
        }

        private void Rooster_OnDie()
        {
            Die();
        }

        protected override void OnRevival()
        {
            _rooster.Revive();
        }
        
        protected override void OnDied() { }
        protected override void OnRestart(bool isFast)
        {
            PrepareRooster();
            _rooster.Restart();
        }

        protected override void OnScoreChanged(int score, int maxScore) { }
        protected override void OnSave() { }

        protected override void OnExit()
        {
            if (_rooster != null)
            {
                _rooster.OnDie -= Rooster_OnDie;
            }

            _referencesHolder.SwipeReceiver.Swiped -= SwipeReceiver_Swiped;
        }
    }
}