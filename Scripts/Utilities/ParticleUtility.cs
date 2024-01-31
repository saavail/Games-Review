using System;
using System.Linq;
using UnityEngine;

namespace Utilities
{
    public static class ParticleUtility
    {
        public static float CalculateMaxLifetime(GameObject particlesParent)
        {
            return CalculateMaxLifetime(particlesParent.GetComponentsInChildren<ParticleSystem>());
        }

        public static float CalculateMaxLifetime(ParticleSystem[] particleSystems)
        {
            if (particleSystems == null || particleSystems.Length == 0)
            {
                throw new Exception($"{typeof(ParticleUtility)} CalculateMaxLifetime: particle systems is null or empty");
            }

            return GetMaxLifetime(particleSystems.Max(p => p.main.startLifetime.constantMax));
        }

        public static float GetMaxLifetime(ParticleSystem particleSystem)
        {
            return particleSystem.main.startLifetime.constantMax;
        }
    }
}