using Sirenix.OdinInspector;
using UnityEngine;

namespace Core
{
    public abstract class GameData : SerializedScriptableObject
    {
        [SerializeField]
        private float _coinsByScore;
        [SerializeField]
        private float _userProgressByScore;
        [SerializeField]
        private PaymentData[] _reviveCosts;  
        
        public float CoinsByScore => _coinsByScore;
        public float UserProgressByScore => _userProgressByScore;
        public PaymentData[] ReviveCosts => _reviveCosts;
    }
}