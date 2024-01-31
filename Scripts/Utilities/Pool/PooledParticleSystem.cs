using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Utilities.Pool
{
    public class PooledParticleSystem : PooledBehaviour
    {
        [SerializeField]
        protected ParticleSystem[] _particleSystems = Array.Empty<ParticleSystem>();

        public override void OnSpawnFromPool()
        {
            base.OnSpawnFromPool();

            if (_particleSystems == null || _particleSystems.Length == 0)
            {
                CalculateFreeTimeout();
            }
            
            foreach (var system in _particleSystems)
            {
                system.Clear();
                system.Stop();

                system.randomSeed = (uint)Random.Range(int.MinValue, int.MaxValue);
                system.Play();
            }
        }

        private void CalculateFreeTimeout()
        {
            // NOTE: вычисляем таймаут исходя из максимальной длительности системы частиц
            _particleSystems = GetComponentsInChildren<ParticleSystem>();

            if (_particleSystems.Length == 0)
            {
                throw new System.Exception($"{typeof(PooledParticleSystem)}: particle systems was not found in gameobject {nameof(gameObject)}");
            }

            FreeTimeout = ParticleUtility.CalculateMaxLifetime(_particleSystems);
        }

        public float GetFreeTimeout()
        {
            if (FreeTimeout == 0)
            {
                CalculateFreeTimeout();
            }
            return FreeTimeout;
        }

#if UNITY_EDITOR
        [ContextMenu("Calculate FreeTimeout")]
        private void BakeFreeTimeout()
        {
            CalculateFreeTimeout();
            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
    }
}
