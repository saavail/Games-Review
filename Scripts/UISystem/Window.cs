using System;
using DependencyInjector;
using DG.Tweening;
using EntryPoint;
using UnityEngine;
using UnityEngine.UI;

namespace UISystem
{
    public abstract class Window : MonoBehaviour
    {
        private const float AnimationDuration = 0.3f;
        
        [SerializeField]
        private Button _backgroundButton;
        [SerializeField]
        private Button _closeButton;
        [SerializeField]
        private RectTransform _contentRect;
        
        private WindowSystem _windowSystem;

        protected virtual bool CanAnimateContent => true;
        protected virtual bool CanAnimate => true;

        public bool IsTweening => (_contentRect != null && DOTween.IsTweening(_contentRect)) 
                                  || (_backgroundButton != null && DOTween.IsTweening(_backgroundButton));
        public bool IsFullClosed => !IsOpen && !IsTweening;
        public bool IsOpen { get; private set; }
        
        protected virtual void Awake()
        {
            if (_backgroundButton != null)
                _backgroundButton.onClick.AddListener(() => Close());
            
            if (_closeButton != null)
                _closeButton.onClick.AddListener(() => Close());
            
            Localize();
        }

        public void Setup(WindowSystem windowSystem)
        {
            _windowSystem = windowSystem;
        }

        public void Open(Action callback = null)
        {
            if (IsTweening && _windowSystem.Current != this)
            {
                AllServices.Container.Single<IDebugConsole>().Post($"cant open window {name}, cause is tweening or not current");
                return;
            }

            IsOpen = true;
            gameObject.SetActive(true);

            if (CanAnimate)
            {
                Sequence sequence = null;
                
                if (_backgroundButton != null)
                {
                    sequence = DOTween.Sequence()
                        .OnComplete(() => callback?.Invoke());
                    
                    sequence.Append(_backgroundButton.image.DOFade(0.65f, AnimationDuration)
                        .From(0f)
                        .SetTarget(_backgroundButton)
                        .SetEase(Ease.OutBack));
                }

                if (CanAnimateContent && _contentRect != null)
                {
                    sequence ??= DOTween.Sequence()
                        .OnComplete(() => callback?.Invoke());

                    sequence.Join(_contentRect.DOScale(Vector3.one, AnimationDuration)
                        .From(Vector3.zero)
                        .SetEase(Ease.OutBack));
                }
            }
            else
            {
                callback?.Invoke();
            }

            OnOpen();
            Refresh();
        }

        protected abstract void OnOpen();
        public abstract void Refresh();
        protected abstract void OnClose();
        protected abstract void Localize();

        public void Close(Action callback = null)
        {
            if (IsTweening || _windowSystem.Current != this)
                return;

            IsOpen = false;

            if (CanAnimate)
            {
                Sequence sequence = DOTween.Sequence()
                    .OnComplete(() =>
                    {
                        gameObject.SetActive(false);
                        callback?.Invoke();
                    });

                if (_backgroundButton != null)
                {
                    sequence.Append(_backgroundButton.image.DOFade(0f, AnimationDuration)
                        .SetTarget(_backgroundButton)
                        .SetEase(Ease.InBack));
                }

                if (CanAnimateContent && _contentRect != null)
                {
                    sequence.Join(_contentRect.DOScale(Vector3.zero, AnimationDuration)
                        .OnComplete(() => gameObject.SetActive(false))
                        .SetEase(Ease.InBack));
                }
            }
            else
            {
                gameObject.SetActive(false);
                callback?.Invoke();
            }

            _windowSystem.OnClose(this);
            OnClose();
        }

        public void ForceClose()
        {
            if (_windowSystem.Current != this)
                return;
            
            if (_contentRect != null)
                _contentRect.DOKill();

            if (_backgroundButton != null)
                _backgroundButton.DOKill();
            
            IsOpen = false;
            gameObject.SetActive(false);
            
            _windowSystem.OnClose(this);
            OnClose();
        }
    }
}