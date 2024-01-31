using System;
using System.Collections.Generic;
using DependencyInjector;
using UnityEngine;

namespace Utilities.Pool
{
    public interface IPoolService : IService
    {
        public event Action<PooledBehaviour, int> EntryInstantiated;
        
        public T Get<T>(T prefab, Vector3? position = null, Quaternion? rotation = null, Vector3? localPosition = null,
            Quaternion? localRotation = null, Transform parent = null, Dictionary<string, object> data = null, bool isReusable = false)
            where T : PooledBehaviour;

        public T Get<T>(T prefab, out bool isNew, Vector3? position = null, Quaternion? rotation = null, Vector3? localPosition = null,
            Quaternion? localRotation = null, Transform parent = null, Dictionary<string, object> data = null, bool isReusable = false)
            where T : PooledBehaviour;

        public bool TryFree(PooledBehaviour obj, bool resetParent = true);
        public void Free(PooledBehaviour obj, bool resetParent = true);
        public void FreeAll(PooledBehaviour prefab = null, bool resetParent = true);
        public void Prepare(PooledBehaviour prefab, PoolGroup group, int count, Transform parent = null, bool forceAddNew = true,
            Dictionary<string, object> data = null);
        public int GetTotalCount(PooledBehaviour prefab = null);
        public int GetFreeCount(PooledBehaviour prefab);
        public Dictionary<PooledBehaviour, int> GetFreeCounts();
        public Dictionary<PooledBehaviour, int> GetBusyCounts();
        public void DestroyAll(PoolGroup group);
        public void DestroyAll(PooledBehaviour prefab);
        public void ApplyToAll(PooledBehaviour prefab, Action<PooledBehaviour> action);
    }
}