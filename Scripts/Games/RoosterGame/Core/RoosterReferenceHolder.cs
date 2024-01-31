using System.Collections.Generic;
using System.Linq;
using Core;
using Games.RoosterGame.Controllers;
using Games.RoosterGame.World;
using UnityEngine;

namespace Games.RoosterGame.Core
{
    public class RoosterReferenceHolder : GameReferencesHolder
    {
        [SerializeField]
        private Map _map;
        [SerializeField]
        private SpeedController _speedController;
        [SerializeField]
        private CoinsController _coinsController;
        [SerializeField]
        private ScoreController _scoreController;
        [SerializeField]
        private Camera _camera;
        [SerializeField]
        private SwipeReceiver _swipeReceiver;
        
        [HideInInspector]
        [SerializeField]
        private MonoBehaviour[] _runnerCreatedHolder;

        private GameObject _controllersHolder;
        
        public List<IRunnerCreated> RunnerCreated { get; private set; }
        public SpeedController SpeedController => _speedController;
        public CoinsController CoinsController => _coinsController;
        public ScoreController ScoreController => _scoreController;
        public Map Map => _map;
        public Camera GameCamera => _camera;
        public SwipeReceiver SwipeReceiver => _swipeReceiver;

        public override void Initialize()
        {
            base.Initialize();
            RunnerCreated = _runnerCreatedHolder.Cast<IRunnerCreated>().ToList();
        }

        public override void CollectData()
        {
            base.CollectData();
            _runnerCreatedHolder = Collect<IRunnerCreated>();

            _camera = FindObjectOfType<Camera>();
            _swipeReceiver = FindObjectOfType<SwipeReceiver>();

            _speedController = FindObjectOfType<SpeedController>();
            _coinsController = FindObjectOfType<CoinsController>();
            _scoreController = FindObjectOfType<ScoreController>();
        }
    }
}