using System;
using Core;

namespace Games.RoosterGame.Controllers
{
    public class ScoreController : BaseController
    {
        private float _playerStartY;

        public int Score { get; private set; }

        public event Action<int> OnScoreChanged; 

        public override void OnStart(GameData gameData)
        {
            base.OnStart(gameData);
            Score = 0;
        }

        public override void OnCreate(Rooster rooster)
        {
            base.OnCreate(rooster);
            _playerStartY = _rooster.transform.position.y;
        }

        public override void OnRestart(bool isRevive)
        {
            if (isRevive)
                return;
            
            Score = 0;
        }

        protected override void OnUpdate()
        {
            int oldScore = Score;
            int newScore = (int)((_rooster.transform.position.y - _playerStartY) * _runnerData.ScoreByDistance);
            Score = newScore;

            if (oldScore != newScore)
            {
                OnScoreChanged?.Invoke(Score);
            }
        }

        protected override void Rooster_OnObstacleCollided(Obstacle obstacle) { }
    }
}