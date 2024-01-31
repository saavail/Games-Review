using Core.PoolStuff;
using Games.RoosterGame.World;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Games.RoosterGame
{
    public class Obstacle : Poolable
    {
        [SerializeField]
        private SpriteRenderer _sprite;
        [SerializeField, OnValueChanged(nameof(OnOvercomingTypeChanged))]
        private OvercomingType _overcomingType;
        [SerializeField]
        private bool _isHighObstacle;

        public bool IsHigh => _isHighObstacle;
        public int SortOrder => _sprite.sortingOrder;

        public OvercomingType OvercomingType => _overcomingType;
        
        public override void FullReset()
        {
        }

        public void SetSortOrder(int order)
        {
            _sprite.sortingOrder = order;
        }

        private void OnOvercomingTypeChanged()
        {
            if (_overcomingType.HasFlag(OvercomingType.Nothing))
            {
                _overcomingType = OvercomingType.Nothing;
            }
            else if (_overcomingType.HasFlag(OvercomingType.Any))
            {
                _overcomingType = OvercomingType.Any;
            }
        }
    }
}