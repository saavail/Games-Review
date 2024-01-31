using System;
using Core;
using Core.UserStuff;
using EntryPoint;
using Enums;
using UISystem;

namespace Games
{
    public class Game2048 : MiniGame, ITableMapEvents
    {
        private new Game2048ReferencesHolder _referencesHolder;
        private TableMap _tableMap;

        private int _maxCellValue = 0;

        public override GameType GameType => GameType.Game2048;
        private new Game2048Data Data { get; set; }
        public int Coins { private get; set; }

        public Game2048(IResourceLoader resourceLoader, ISceneLoaderService sceneLoaderService, WindowSystem windowSystem, IUserService userService)
            : base(resourceLoader, sceneLoaderService, windowSystem, userService) { }

        protected override void ShowTutorial()
        {
            OnTutorialShowed();
        }
        
        public override void OnTutorialShowed()
        {
            base.OnTutorialShowed();
            OnStart();
        }

        protected override void OnStart()
        {
            _referencesHolder = (Game2048ReferencesHolder) base._referencesHolder;
            Data = (Game2048Data) base.Data;
            
            _tableMap = new TableMap(4, Data.Cell4Chance, new ITableMapEvents[]
            {
                _referencesHolder.TableMapView,
                this,
            }, Data.CanLog);
            
            _referencesHolder.SwipeReceiver.SetTurnCooldown(Data.TurnCooldown);
            _referencesHolder.SwipeReceiver.Swiped += SwipeReceiver_Swiped;

            _maxCellValue = Data.StartScoreValue;
        }

        protected override void OnDied() { }

        protected override void OnRestart(bool isFast)
        {
            Coins = 0;
            _tableMap.RestartGame();
            _maxCellValue = Data.StartScoreValue;
        }

        protected override void OnRevival()
        {
            _tableMap.Revive();
        }

        protected override void OnScoreChanged(int score, int maxScore) { }

        protected override void OnSave() { }

        protected override void OnExit()
        {
            _referencesHolder.SwipeReceiver.Swiped -= SwipeReceiver_Swiped;
        }

        public override int CalculateCoins()
        {
            return (int)Math.Round(Data.CoinsByScore * Coins, MidpointRounding.AwayFromZero);
        }

        void ITableMapEvents.OnPush(int x, int y, int value) { }

        void ITableMapEvents.OnMove(int x1, int y1, int x2, int y2) { }

        void ITableMapEvents.OnMerge(int x1, int y1, int x2, int y2, int value)
        {
            if (value > _maxCellValue)
            {
                Coins++;
                _maxCellValue = value;
            }
            
            SetScore(Score + value);
        }

        void ITableMapEvents.OnDelete(int x, int y) { }

        private void SwipeReceiver_Swiped(Swipe swipe)
        {
            if (_tableMap.IsLose())
                return;
            
            _tableMap.Swipe(swipe);
            
            if (_tableMap.IsLose())
            {
                Die();
            }
        }
    }
}