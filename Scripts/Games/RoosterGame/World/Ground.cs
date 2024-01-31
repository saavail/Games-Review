using System.Collections.Generic;
using System.Linq;
using Core;
using Core.PoolStuff;
using EntryPoint;
using Games.RoosterGame.Core;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Games.RoosterGame.World
{
    public class Ground : Poolable, IDataCollectable
    {
        [SerializeField]
        private SpriteRenderer _spriteRenderer;
        [SerializeField, HideInInspector]
        private Obstacle[] _obstacles;
        [SerializeField, HideInInspector]
        private CoinPlace[] _coinPLaces;

        private List<CoinObstacle> _coins = new();
        
        public Bounds Bounds => _spriteRenderer.bounds;

        public override void FullReset() { }

        public void SpawnCoins(Pool<CoinObstacle> coinsPool)
        {
            foreach (var coinPLace in _coinPLaces)
            {
                CoinObstacle coin = coinsPool.Get();
                coin.transform.SetParent(transform);
                coin.transform.position = coinPLace.Position;
                
                coin.Initialize(coinPLace.Value);
                _coins.Add(coin);
            }
        }

        public void UnParentCoins()
        {
            foreach (var coin in _coins) 
                coin.Release();

            _coins.Clear();
        }
        
        [Button]
        public void CollectData()
        {
            _coinPLaces = GetComponentsInChildren<CoinPlace>();
            _obstacles = GetComponentsInChildren<Obstacle>();

            var map = SceneManager.GetSceneByName("RoosterRunner")
                .GetRootGameObjects()
                .FirstOrDefault(i => i.GetComponent<Map>() != default)
                ?.GetComponent<Map>();
            
            if (map == default)
                return;

            foreach (var obstacle in _obstacles)
            {
                var position = obstacle.transform.position;
                var lineIndex = map.GetLineIndex(position);

                position.x = map[lineIndex];
                obstacle.transform.position = position;
            }
        }

        public bool TryDestroyCoin(CoinObstacle coin)
        {
            if (_coins.Contains(coin))
            {
                coin.Release();
                _coins.Remove(coin);
                return true;
            }

            return false;
        }
    }
}