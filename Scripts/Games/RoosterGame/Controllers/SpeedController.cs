namespace Games.RoosterGame.Controllers
{
    public class SpeedController : BaseController
    {
        private float _playerStartY;

        public float Speed { get; private set; }
        
        public override void OnCreate(Rooster rooster)
        {
            base.OnCreate(rooster);
            _playerStartY = _rooster.transform.position.y;
        }

        public override void OnRestart(bool isRevive)
        {
            if (isRevive)
                return;
            
            Speed = _runnerData.BaseSpeed;
        }

        protected override void OnUpdate()
        {
            float distance = _rooster.transform.position.y - _playerStartY;
            float distanceNormalized = distance / _runnerData.MaxDistanceForSpeedFactor;
            Speed = _runnerData.BaseSpeed + _runnerData.SpeedByDistance.Evaluate(distanceNormalized) * _runnerData.SpeedFactor;
        }

        protected override void Rooster_OnObstacleCollided(Obstacle obstacle) { }
    }
}