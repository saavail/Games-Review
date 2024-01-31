using Core;
using Core.UserStuff;
using EntryPoint;
using Enums;
using UISystem;
using UnityEngine;

namespace Games.Stack.Core
{
    public class GameStack : MiniGame
    {

        private new StackReferencesHolder _referencesHolder;
        public override GameType GameType => GameType.Stack;
        
        private new GameStackData Data { get; set; }
        
        public GameStack(IResourceLoader resourceLoader, ISceneLoaderService sceneLoaderService,
            WindowSystem windowSystem, IUserService userService)
            : base(resourceLoader, sceneLoaderService, windowSystem, userService) { }
        
        protected override void ShowTutorial() { }

        protected override void OnStart() { }

        protected override void OnRevival() { }

        protected override void OnDied() { }

        protected override void OnRestart(bool isFast) { }

        protected override void OnScoreChanged(int score, int maxScore) { }

        protected override void OnSave() { }

        protected override void OnExit() { }
    }
}