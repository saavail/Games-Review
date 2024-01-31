using Core;
using Games.RoosterGame.Core;
using UnityEngine;

namespace Games.RoosterGame.Controllers
{
    public abstract class BaseController : MonoBehaviour, IGameStartable, IRunnerCreated, IGameExitable, IGameRestartable
    {
        protected RoosterRunnerData _runnerData;
        protected Rooster _rooster;
        
        public virtual void OnStart(GameData gameData)
        {
            _runnerData = (RoosterRunnerData)gameData;
        }

        public virtual void OnCreate(Rooster rooster)
        {
            _rooster = rooster;
            _rooster.OnObstacleCollided += Rooster_OnObstacleCollided;
        }

        public virtual void OnExit()
        {
            _rooster.OnObstacleCollided -= Rooster_OnObstacleCollided;
        }

        public abstract void OnRestart(bool isRevive);

        private void Update()
        {
            if (_rooster.IsDead)
                return;
            
            OnUpdate();
        }

        protected abstract void OnUpdate();

        protected abstract void Rooster_OnObstacleCollided(Obstacle obstacle);
    }
}