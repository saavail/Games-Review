using System;
using UnityEngine;

namespace Games
{
    [Serializable]
    public class CellColor
    {
        [SerializeField]
        private Color _color = Color.white;

        [SerializeField]
        private int _value = 2;

        public Color Color => _color;
        public int Value => _value;
    }
}