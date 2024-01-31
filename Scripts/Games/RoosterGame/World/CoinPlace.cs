using UnityEngine;

namespace Games.RoosterGame.World
{
    public class CoinPlace : MonoBehaviour
    {
        public int Value;
        
        [HideInInspector]
        public CoinObstacle CoinObstacle;
        
        public Vector3 Position => transform.position;
        
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(transform.position, 0.5f);
        }
    }
}