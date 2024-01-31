using System;

namespace Games.RoosterGame.Controllers
{
    public class CoinsController : BaseController
    {
        public int Coins { get; private set; }
        
        public event Action<int> OnCoinsChanged;

        public override void OnRestart(bool isRevive)
        {
            if (isRevive)
                return;
            
            Coins = 0;
        }

        protected override void OnUpdate() { }

        protected override void Rooster_OnObstacleCollided(Obstacle obstacle)
        {
            if (obstacle.OvercomingType is not OvercomingType.Any)
                return;
            
            if (obstacle is not CoinObstacle coinObstacle)
                return;

            Coins += coinObstacle.Coins;
            OnCoinsChanged?.Invoke(Coins);
        }
    }
}