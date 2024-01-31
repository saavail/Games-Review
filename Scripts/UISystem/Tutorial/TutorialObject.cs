using DG.Tweening;
using UnityEngine;

namespace UISystem.Tutorial
{
    public class TutorialObject : MonoBehaviour, ITutorialObject
    {
        private const float AnimationDuration = 0.3f;
        
        [SerializeField]
        private RectTransform _contentRect;

        private Sequence _sequence;

        public void Show()
        {
            gameObject.SetActive(true);
            
            _sequence = DOTween.Sequence();

            if (_contentRect != null)
            {
                _sequence.Append(_contentRect.DOScale(Vector3.one, AnimationDuration)
                    .From(Vector3.zero)
                    .SetEase(Ease.OutBack));

                _sequence.Play();
            }
        }
    }
}