using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Games
{
    public class SwipeReceiver : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        private float _turnCooldown;
        private float _lastSwipeTime;
        
        private Vector2 _startInput;

        public event Action<Swipe> Swiped;

        public void SetTurnCooldown(float cooldown)
        {
            _turnCooldown = cooldown;
        }
        
        public void OnBeginDrag(PointerEventData eventData)
        {
            _startInput = eventData.position;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (Time.realtimeSinceStartup - _lastSwipeTime < _turnCooldown)
                return;
            
            var angle = Mathf.Atan2(eventData.position.y - _startInput.y, eventData.position.x - _startInput.x) * Mathf.Rad2Deg;

            switch (angle)
            {
                case >= 45 and <= 135:
                    Swiped?.Invoke(Swipe.Up);
                    break;
                
                case <= 45 and >= -45:
                    Swiped?.Invoke(Swipe.Right);
                    break;
                
                case <= -45 and >= -135:
                    Swiped?.Invoke(Swipe.Down);
                    break;
                
                case >= 135 or <= -135:
                    Swiped?.Invoke(Swipe.Left);
                    break;
            }

            _lastSwipeTime = Time.realtimeSinceStartup;
        }

        public void OnDrag(PointerEventData eventData) { }
    }
}