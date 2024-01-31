using System.Collections.Generic;
using System.Linq;
using Core;
using Games.RoosterGame.World;
using UnityEngine;
using UnityEngine.Serialization;

namespace Games.RoosterGame.Core
{
    [CreateAssetMenu(menuName = "Scriptables/GameData/" + nameof(RoosterRunnerData), fileName = nameof(RoosterRunnerData))]
    public class RoosterRunnerData : GameData
    {
        [Header("Runner")]
        [SerializeField]
        private float _baseSpeed;
        [SerializeField]
        private float _speedFactor;
        [SerializeField]
        private float _maxDistanceForSpeedFactor;
        [SerializeField]
        private AnimationCurve _speedByDistance;
        [SerializeField]
        private float _switchLineSpeed;
        [SerializeField]
        private List<RoosterActionData> _runnerActionsData;
        [SerializeField]
        private float _godModeDurationOnRevive;
        
        [SerializeField]
        private float _scoreByDistance;
        
        [FormerlySerializedAs("_groundPrefab")]
        [Header("Prefabs")]
        [SerializeField]
        private Ground[] _groundPrefabs;
        [SerializeField]
        private CoinObstacle _coinObstaclePrefab;
        [SerializeField]
        private Rooster _roosterPrefab;

        public float BaseSpeed => _baseSpeed;
        public float SpeedFactor => _speedFactor;
        public float MaxDistanceForSpeedFactor => _maxDistanceForSpeedFactor;
        public AnimationCurve SpeedByDistance => _speedByDistance;
        public float SwitchLineSpeed => _switchLineSpeed;
        public List<RoosterActionData> RunnerActionsData => _runnerActionsData;
        public float GodModeDurationOnRevive => _godModeDurationOnRevive;
        
        public float ScoreByDistance => _scoreByDistance;
        public Ground[] GroundPrefabs => _groundPrefabs;
        public CoinObstacle CoinObstaclePrefab => _coinObstaclePrefab;
        public Rooster RoosterPrefab => _roosterPrefab;

        public RoosterActionData GetActionData(OvercomingType type) => 
            _runnerActionsData.FirstOrDefault(i => i.Type == type);
    }
}