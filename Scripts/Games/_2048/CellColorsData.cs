using System;
using System.Linq;
using UnityEngine;

namespace Games
{
    [Serializable]
    public class CellColorsData
    {
        [SerializeField]
        private CellColor[] _cellColors;

        public CellColor[] CellColors => _cellColors;

        public Color ColorByValue(int value)
        {
            value = Mathf.Clamp(value, _cellColors[0].Value, _cellColors[^1].Value);
            return _cellColors.FirstOrDefault(i => i.Value == value)?.Color ?? Color.white;
        }
    }
}