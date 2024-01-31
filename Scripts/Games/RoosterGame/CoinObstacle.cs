namespace Games.RoosterGame
{
    public class CoinObstacle : Obstacle
    {
        public int Coins { get; private set; }

        public void Initialize(int coins)
        {
            Coins = coins;
        }
    }
}