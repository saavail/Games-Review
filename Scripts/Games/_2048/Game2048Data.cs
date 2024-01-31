using System;
using System.Linq;
using Core;
using Enums;
using UnityEngine;

namespace Games
{
    [CreateAssetMenu(menuName = "Scriptables/GameData/" + nameof(Game2048Data), fileName = nameof(Game2048Data))]
    public class Game2048Data : GameData
    {
        [Serializable]
        private class SkinColors
        {
            [SerializeField]
            private SkinType _skinType;
            [SerializeField]
            private CellColorsData _cellColors;
            
            public CellColorsData CellColors => _cellColors;
            public SkinType SkinType => _skinType;
        }
        
        [SerializeField]
        private int _startScoreValue;

        [SerializeField]
        private float _cell4Chance;

        [SerializeField]
        private float _turnCooldown;

        [SerializeField]
        private SkinColors[] _skinColors;

        [SerializeField]
        private bool _canLog;

        public int StartScoreValue => _startScoreValue;
        public float Cell4Chance => _cell4Chance;
        public float TurnCooldown => _turnCooldown;
        public bool CanLog => _canLog;

        public CellColorsData ColorsBySkin(SkinType skinType)
            => _skinColors.FirstOrDefault(i => i.SkinType == skinType)?.CellColors;
    }
}