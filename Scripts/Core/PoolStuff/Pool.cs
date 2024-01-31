using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Core.PoolStuff
{
    public class Pool<T>
        where T : Poolable, new()
    {
        private readonly T _poolable;
        private readonly Stack<T> _freeObjects = new();

        private readonly Transform _parent;

        public Pool(string resourcePath, int prepareCount = 0, Transform parent = null) 
            : this(Resources.Load<T>(resourcePath), prepareCount, parent)
        {
        }

        public Pool(T poolable, int prepareCount = 0, Transform parent = null)
        {
            _poolable = poolable;
            
            var list = new int[prepareCount].Select(i => Get()).ToArray();

            foreach (var temp in list) 
                temp.Release();

            _parent = parent;
        }

        public T Get()
        {
            if (_freeObjects.Count > 0)
            {
                var poolable = _freeObjects.Pop();
                
                poolable.gameObject.SetActive(true);
                poolable.FullReset();

                if (_parent != null && poolable.transform.parent != _parent)
                {
                    poolable.transform.SetParent(_parent);
                }
                
                return poolable;
            }
            else
            {
                var poolable = Object.Instantiate(_poolable);
                poolable.SetReleaseAction(() => Release(poolable));
                
                if (_parent != null)
                {
                    poolable.transform.SetParent(_parent);
                }
                
                return poolable;
            }
        }

        public void Release(T poolable)
        {
            if (_freeObjects.Contains(poolable))
                return;
            
            poolable.gameObject.SetActive(false);
            poolable.transform.SetParent(null);
            _freeObjects.Push(poolable);
        }

        public void ReleaseWithDelay(T poolable, float seconds)
        {
            poolable.ReleaseWithDelay(seconds);
        }
    }
}