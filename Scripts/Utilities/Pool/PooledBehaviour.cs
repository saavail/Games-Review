using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities.Pool
{
    public class PooledBehaviour : MonoBehaviour
    {
        public static event Action<PooledBehaviour> Spawned = delegate { };
        public event Action<PooledBehaviour> Returned = delegate { };

        public event Action InstanceSpawned = null;
        public event Action InstanceReturned = null;

        [SerializeField]
        private float _freeTimeout = 0f;

        [SerializeField]
        protected PoolGroup _group = PoolGroup.DestroyOnBattleEnd;

        private Coroutine _freeTimeoutCoroutine = null;

        public Dictionary<string, object> _data = null;
        public Dictionary<string, object> _prepareData = null;

        private bool _isPrepared = false;
        
        public float FreeTimeout
        {
            get
            {
                return _freeTimeout;
            }

            set
            {
                _freeTimeout = value;

                if (gameObject.activeInHierarchy)
                {
                    HandleFreeTimeoutChanged();
                }   
            }
        }

        public virtual PoolGroup Group
        {
            get
            {
                return _group;
            }

            set
            {
                _group = value;
            }
        }
                
        public void Prepare(Dictionary<string, object> data)
        {
            if (!_isPrepared)
            {
                _prepareData = data;
                OnPreparePool();
                _isPrepared = true;
            }
        }

        protected virtual void OnPreparePool()
        {
            
        }

        public virtual void OnSpawnFromPool()
        {
            HandleFreeTimeoutChanged();

            if (InstanceSpawned != null)
            {
                InstanceSpawned();
                InstanceSpawned = null;
            }

            Spawned(this);
        }

        public virtual bool BeforeReturnToPool()
        {
            return true;
        }

        public virtual void OnReturnToPool()
        {
            if (InstanceReturned != null)
            {
                InstanceReturned();
                InstanceReturned = null;
            }

            Returned(this);
        }

        private void HandleFreeTimeoutChanged()
        {
            if (_freeTimeoutCoroutine != null)
            {
                StopCoroutine(_freeTimeoutCoroutine);
                _freeTimeoutCoroutine = null;
            }

            if (_freeTimeout > 0.01f)
            {
                _freeTimeoutCoroutine = this.InvokeWithDelay(_freeTimeout, () => 
                {
                    // NOTE: TNK-994 - периодически, мы попадаем сюда для уже освобождённого объекта, поэтому используем TryFree(), чтобы избежать ошибок.
                    //Pool.Instance.TryFree(this);
                    Debug.LogException(new Exception("Not method implementation, delete comment or write new logic for DI"));
                });
            }   
        }
        
        public void SetData(Dictionary<string, object> data)
        {
            _data = data;
        }

        public void ClearData()
        {
            _data = null;
        }

        protected T GetPrepareDataValue<T>(string itemKey, T defaultValue = default)
        {
            return GetDataValue<T>(itemKey, defaultValue, _prepareData);
        }

        protected T GetDataValue<T>(string itemKey, T defaultValue = default, Dictionary<string, object> forcedData = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>(forcedData ?? _data);

            if (data == null || data.Count == 0)
            {
                return defaultValue;
            }

            if (!data.TryGetValue(itemKey, out object itemObject))
            {
                return defaultValue;
            }

            if (itemObject is T)
            {
                return (T)itemObject;
            }

            return defaultValue;
        }
    }
}