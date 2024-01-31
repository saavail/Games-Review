using System;
using Games.RoosterGame.World;
using UnityEngine;

namespace Games.RoosterGame.Core
{
    [Serializable]
    public class RoosterActionData
    {
        [SerializeField]
        private OvercomingType _type;
        [SerializeField]
        private float _duration;

        public OvercomingType Type => _type;
        public float Duration => _duration;
    }
}