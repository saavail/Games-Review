using System.Collections.Generic;
using Core;
using Core.PoolStuff;
using Games.RoosterGame.Core;
using Unity.VisualScripting;
using UnityEngine;

namespace Games.RoosterGame.World
{
    public class MapScroller : MonoBehaviour, IGameStartable, IRunnerCreated, IGameRestartable, IGameExitable
    {
        [SerializeField]
        private Transform _groundParent;
        [SerializeField]
        private Ground _startGround;
        
        private Rooster _rooster;

        private RoosterRunnerData _data;
        private readonly Queue<Ground> _grounds = new();
        private Vector2 _groundSize;
        private Bounds _groundBounds;

        private Pool<CoinObstacle> _coinsPool;
        
        public float PlayerPositionY => _rooster.transform.position.y;
        
        void IGameStartable.OnStart(GameData gameData)
        {
            _data = (RoosterRunnerData)gameData;

            _startGround.SetReleaseAction(() => _startGround.gameObject.SetActive(false));
            _groundSize = _startGround.Bounds.size;
            _startGround.transform.position = Vector3.zero;
            
            _groundBounds = new Bounds(Vector3.zero, new Vector3(_groundSize.x, _groundSize.y, 0));
            _grounds.Enqueue(_startGround);

            _coinsPool = new Pool<CoinObstacle>(_data.CoinObstaclePrefab, 3, _groundParent);
        }
        
        private void LateUpdate()
        {
            if (_rooster == null || _rooster.IsDead)
                return;
            
            if (!_groundBounds.Contains(Vector3.up * (PlayerPositionY + 40)))
            {
                Ground ground = CreateGround();
                ground.SpawnCoins(_coinsPool);
                _grounds.Enqueue(ground);
                ground.transform.position = Vector3.up * (_groundBounds.max.y + _groundSize.y / 2f);
                
                _groundBounds.max += Vector3.up * _groundSize.y;
            }

            if (_grounds.Peek().Bounds.min.y < PlayerPositionY - 50)
            {
                _grounds.Dequeue().Release();
                _groundBounds.min -= Vector3.up * _groundSize.y;
            }
        }

        private Ground CreateGround()
        {
            Ground ground = Instantiate(_data.GroundPrefabs[Random.Range(0, _data.GroundPrefabs.Length)], _groundParent);
            
            ground.SetReleaseAction(() =>
            {
                ground.UnParentCoins();
                Destroy(ground.gameObject);
            });
            
            return ground;
        }

        void IRunnerCreated.OnCreate(Rooster rooster)
        {
            _rooster = rooster;
            _rooster.OnObstacleCollided += Rooster_OnObstacleCollided;
        }

        private void Rooster_OnObstacleCollided(Obstacle obstacle)
        {
            if (obstacle.OvercomingType is not OvercomingType.Any)
                return;
            
            if (obstacle is not CoinObstacle coin)
                return;
            
            foreach (var ground in _grounds)
            {
                if (ground.TryDestroyCoin(coin))
                    break;
            }
        }

        void IGameRestartable.OnRestart(bool isRevive)
        {
            if (isRevive)
                return;
            
            foreach (var ground in _grounds) 
                ground.Release();
            
            _grounds.Clear();
            _grounds.Enqueue(_startGround);
            _startGround.gameObject.SetActive(true);
            
            _groundBounds = new Bounds(Vector3.zero, new Vector3(_groundSize.x, _groundSize.y, 0));
        }

        void IGameExitable.OnExit()
        {
            _rooster.OnObstacleCollided -= Rooster_OnObstacleCollided;
        }
    }
}