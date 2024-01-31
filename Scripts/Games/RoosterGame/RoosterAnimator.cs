using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

namespace Games.RoosterGame
{
    public class RoosterAnimator : MonoBehaviour
    {
        private static readonly int IsJumping = Animator.StringToHash("IsJumping");
        private static readonly int IsSliding = Animator.StringToHash("IsSliding");
        private static readonly int IsSwapping = Animator.StringToHash("IsSwapping");
        private static readonly int IsDead = Animator.StringToHash("IsDead");
        
        [SerializeField]
        private Rooster _rooster;
        [SerializeField]
        private Animator _animator;
        [SerializeField]
        private SpriteRenderer _sprite;

        [Header("Settings")]
        [SerializeField]
        private float _godModeStepDuration;
        [SerializeField]
        private float _jumpScale;
        [SerializeField]
        private float _returnToNormalScaleDuration = 0.15f;

        private Coroutine _godModeRoutine;

        private void Awake()
        {
            _rooster.OnActionChanged += Rooster_OnActionChanged;
            _rooster.OnObstacleCollided += Rooster_OnObstacleCollided;
            _rooster.OnSideMove += Rooster_OnSideMove;
            _rooster.OnDie += Rooster_OnDie;
            _rooster.OnRevive += Rooster_OnRevive;
        }

        private void OnDestroy()
        {
            _rooster.OnActionChanged -= Rooster_OnActionChanged;
            _rooster.OnObstacleCollided -= Rooster_OnObstacleCollided;
            _rooster.OnSideMove -= Rooster_OnSideMove;
            _rooster.OnDie -= Rooster_OnDie;
            _rooster.OnRevive -= Rooster_OnRevive;
        }

        private void Rooster_OnActionChanged(OvercomingType type, float time)
        {
            _sprite.sortingOrder = type == OvercomingType.Roll
                ? RoosterStaticData.RoosterRollSortOrder
                : RoosterStaticData.RoosterSortOrder;
            
            switch (type)
            {
                case OvercomingType.Nothing:
                {
                    _animator.SetBool(IsJumping, false);
                    _animator.SetBool(IsSliding, false);
                }
                    break;
                
                case OvercomingType.Roll:
                {
                    ReturnToNormalScale();
                    _animator.SetBool(IsSliding, true);
                    _animator.SetBool(IsJumping, false);
                }
                    break;
                
                case OvercomingType.Jump:
                {
                    _animator.SetBool(IsJumping, true);
                    _animator.SetBool(IsSliding, false);

                    _sprite.transform.DOKill();

                    DOTween.Sequence(_sprite.transform)
                        .Append(_sprite.transform.DOScale(Vector3.one * _jumpScale, time / 2)
                            .SetEase(Ease.OutQuad))
                        .Append(_sprite.transform.DOScale(Vector3.one, time / 2)
                            .SetEase(Ease.InQuad));
                }
                    break;
            }
        }

        private void ReturnToNormalScale()
        {
            if (DOTween.IsTweening(_sprite.transform))
            {
                _sprite.transform.DOKill();
                _sprite.transform.DOScale(Vector3.one, _returnToNormalScaleDuration);
            }
        }

        private void Rooster_OnObstacleCollided(Obstacle obstacle)
        {
        }

        private void Rooster_OnSideMove(int direction)
        {
            _animator.SetBool(IsSwapping, direction != 0);
            _sprite.flipX = direction > 0;
        }

        private void Rooster_OnDie()
        {
            ReturnToNormalScale();
            _animator.SetBool(IsDead, true);
            _sprite.sortingOrder = RoosterStaticData.RoosterSortOrder;
        }

        private void Rooster_OnRevive(float duration)
        {
            _animator.SetBool(IsDead, false);
            _animator.SetBool(IsJumping, false);
            _animator.SetBool(IsSliding, false);
            _animator.SetBool(IsSwapping, false);
            
            if (duration < 0)
                return;

            StopGodMode();
            _godModeRoutine = StartCoroutine(GodModeRoutine(duration));
        }

        private void StopGodMode()
        {
            if (_godModeRoutine != null)
            {
                StopCoroutine(_godModeRoutine);
                _godModeRoutine = null;
                _sprite.enabled = true;
            }
        }

        private IEnumerator GodModeRoutine(float duration)
        {
            float stepTime = _godModeStepDuration;

            while (duration > 0)
            {
                yield return null;
                duration -= Time.deltaTime;

                stepTime -= Time.deltaTime;

                if (stepTime < 0)
                {
                    stepTime = _godModeStepDuration;
                    _sprite.enabled = !_sprite.enabled;
                }
            }

            _sprite.enabled = true;
            _godModeRoutine = null;
        }
    }
}