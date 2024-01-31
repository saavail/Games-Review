using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UISystem
{
    public class BackendWaiter : MonoBehaviour, IBackendWaiter
    {
        private const float AnimationDuration = 0.35f;
        private const float BlurAmount = 3f;
        
        private static readonly int Size = Shader.PropertyToID("_Size");

        [SerializeField]
        private Image _background;
        [SerializeField]
        private Image _loadIcon;
        [SerializeField]
        private Transform _loadIconRoot;
        [SerializeField]
        private TextMeshProUGUI _descriptionHintText;
        [SerializeField]
        private Animator _coinAnimator;


        public bool IsActive => gameObject.activeSelf;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            _descriptionHintText.text = Strings.CommonLoading;
        }

        [Button]
        public void Show()
        {
            Kill();
            SetActive(true);

            // _background.DOFade(1f, AnimationDuration)
            //     .SetEase(Ease.OutSine)
            //     .From(0f);

            float duration = GetDynamicDuration(true);

            DOTween.To(() => duration, value =>
            {
                _background.material.SetFloat(Size, value);
            }, BlurAmount, duration)
                .From(0f)
                .SetEase(Ease.OutSine)
                .SetTarget(_background);

            _loadIconRoot.DOScale(Vector3.one, duration)
                .SetEase(Ease.OutBack)
                .From(Vector3.zero);
            
            // _loadIcon.DOFade(1f, AnimationDuration)
            //     .SetEase(Ease.OutSine)
            //     .From(0f);

            _descriptionHintText.DOFade(1f, duration)
                .SetEase(Ease.OutSine)
                .From(0f);
            
            CreateLoadingAnimation(_descriptionHintText, 100, Strings.CommonLoading, 0.5f);
        }

        private float GetDynamicDuration(bool isShow)
        {
            return isShow
                ? AnimationDuration - (_background.material.GetFloat(Size) / BlurAmount)
                : AnimationDuration - (1 - _background.material.GetFloat(Size) / BlurAmount);
        }

        private void CreateLoadingAnimation(TextMeshProUGUI id, float duration, string text, float dotTimeStep, int dotCount = 3)
        {
            string dots = new('.', dotCount);
            DOTween.To(() => duration, value =>
            {
                id.text = text + dots[..(int)((duration - value) / dotTimeStep % (dotCount + 1))];
            }, 0f, duration)
                .SetEase(Ease.Linear)
                .SetTarget(_descriptionHintText.transform);
        }

        public void ShowException()
        {
            // todo: показывать как микро всплывающие смс с красным восклицательным 
        }

        [Button]
        public void Hide()
        {
            Kill();

            float duration = GetDynamicDuration(false);

            // _background.DOFade(0f, AnimationDuration)
            //     .SetEase(Ease.InSine)
            //     .OnComplete(() => SetActive(false));
            
            DOTween.To(() => duration, value =>
                {
                    _background.material.SetFloat(Size, value);
                }, 0f, duration)
                .SetEase(Ease.InSine)
                .SetTarget(_background);

            _loadIconRoot.transform.DOScale(Vector3.zero, duration)
                .SetEase(Ease.InBack);

            _descriptionHintText.DOFade(0f, duration)
                .SetEase(Ease.InSine)
                .OnComplete(() => SetActive(false));
        }

        public void ForceHide()
        {
            Kill();
            SetActive(false);
        }

        private void Kill()
        {
            DOTween.Kill(_background);
            DOTween.Kill(_loadIconRoot);
            DOTween.Kill(_descriptionHintText);
            DOTween.Kill(_descriptionHintText.transform);
        }

        private void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);
        }
    }
}