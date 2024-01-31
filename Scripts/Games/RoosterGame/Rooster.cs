using System;
using Games.RoosterGame.Controllers;
using Games.RoosterGame.Core;
using Games.RoosterGame.World;
using UnityEngine;

namespace Games.RoosterGame
{
    public class Rooster : MonoBehaviour
    {
        private int _lineIndex;
        private int _movingToLineIndex;

        private OvercomingType _overcomingType = OvercomingType.Nothing;
        private float _actionTime;
        private float _godModeTime;

        private Map _map;
        private SpeedController _speedController;
        private RoosterRunnerData _runnerData;

        public event Action<Obstacle> OnObstacleCollided;
        public event Action OnDie;
        public event Action<float> OnRevive;
        public event Action<OvercomingType, float> OnActionChanged;
        public event Action<int> OnSideMove;

        public bool IsAlive { private set; get; }
        public bool IsDead => !IsAlive;
        public bool IsInGodMode => _godModeTime > 0;

        public void Initialize(Map map, SpeedController speedController, RoosterRunnerData runnerData)
        {
            IsAlive = true;
            _map = map;
            _speedController = speedController;
            _runnerData = runnerData;

            InitializePositionIndexes(1);
        }

        public void InitializePositionIndexes(int lineIndex)
        {
            _lineIndex = lineIndex;
            _movingToLineIndex = lineIndex;
        }

        public void Update()
        {
            if (IsDead)
                return;

            float newX = transform.position.x;
            float newY = transform.position.y + _speedController.Speed * Time.deltaTime;

            if (_lineIndex != _movingToLineIndex)
            {
                float targetX = _map[_movingToLineIndex];
                float direction = Math.Sign(targetX - transform.position.x);
                float deltaX = _runnerData.SwitchLineSpeed * Time.deltaTime * direction;

                if (_movingToLineIndex == 1)
                {
                    newX = direction < 0 
                        ? Mathf.Max(newX + deltaX, 0) 
                        : Mathf.Min(newX + deltaX, 0);
                }
                else if (_movingToLineIndex == 2)
                {
                    newX = Mathf.Min(newX + deltaX, _map[2]);
                }
                else
                {
                    newX = Mathf.Max(newX + deltaX, _map[0]);
                }
            }

            transform.position = new Vector3(newX, newY, 0);

            if (_map.IsAbsolutelyInLine(transform.position, _movingToLineIndex))
            {
                _lineIndex = _movingToLineIndex;
                OnSideMove?.Invoke(0);
            }

            if (_overcomingType is not OvercomingType.Nothing)
            {
                _actionTime -= Time.deltaTime;

                if (_actionTime < 0)
                {
                    ChangeAction(OvercomingType.Nothing);
                }
            }

            if (IsInGodMode)
            {
                _godModeTime -= Time.deltaTime;
            }
        }

        public void HandleSwipe(Swipe swipe)
        {
            switch (swipe)
            {
                case Swipe.Left:
                    ChangeMoveSide(-1);
                    break;
                case Swipe.Right:
                    ChangeMoveSide(1);
                    break;
                case Swipe.Up:
                    ChangeAction(OvercomingType.Jump);
                    break;
                case Swipe.Down:
                    ChangeAction(OvercomingType.Roll);
                    break;
            }
        }

        private void ChangeMoveSide(int deltaLine)
        {
            if (_movingToLineIndex != _lineIndex)
                return;
            
            if (_lineIndex + deltaLine < 0 || _lineIndex + deltaLine >= _map.LinesCount)
                return;

            _movingToLineIndex = _lineIndex + deltaLine;
            OnSideMove?.Invoke(_movingToLineIndex - _lineIndex);
        }

        private void ChangeAction(OvercomingType type)
        {
            if (type == _overcomingType)
                return;
            
            _overcomingType = type;
            _actionTime = _runnerData.GetActionData(type)?.Duration ?? -1f;
            
            OnActionChanged?.Invoke(_overcomingType, _actionTime);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (IsDead)
                return;
            
            var obstacle = other.gameObject.GetComponent<Obstacle>();

            if (obstacle == null)
                return;

            if (obstacle.OvercomingType is not OvercomingType.Any
                && _overcomingType is not OvercomingType.Any
                && !IsInGodMode
                && (obstacle.OvercomingType == OvercomingType.Nothing || !obstacle.OvercomingType.HasFlag(_overcomingType)))
            {
                IsAlive = false;
                OnDie?.Invoke();
                return;
            }
            
            OnObstacleCollided?.Invoke(obstacle);
        }

        public void Revive()
        {
            _godModeTime = _runnerData.GodModeDurationOnRevive;
            IsAlive = true;
            OnRevive?.Invoke(_runnerData.GodModeDurationOnRevive);
        }

        public void Restart()
        {
            IsAlive = true;
            _actionTime = 0;
            _godModeTime = 0;
            _overcomingType = OvercomingType.Nothing;
            OnRevive?.Invoke(-1);
        }
    }
}