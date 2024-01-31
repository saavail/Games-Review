using System;
using DG.Tweening;
using Extensions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Utilities
{
    public class PrettyButton : Button
    {
        [SerializeField]
        private CanvasRenderer[] _ignoreRenderers = Array.Empty<CanvasRenderer>();
        [SerializeField]
        private CanvasRenderer[] _renderers = Array.Empty<CanvasRenderer>();
    
        [Header("Bounce Animation")]
        [SerializeField]
        private bool _haveBounceAnimation;
        [SerializeField]
        private float _targetScaleFactor = 0.85f;
        [SerializeField]
        private float _animationTime = 0.15f;

        private bool _state = true;
        private Vector3 _defaultScale;

        private bool _isPointerDown;
        private bool _isPointerInside;
    
        private int Id => GetInstanceID();

        protected override void Awake()
        {
            base.Awake();
            _defaultScale = transform.localScale;
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            _isPointerInside = false;
            _isPointerDown = false;
        
            if (_haveBounceAnimation)
                transform.localScale = _defaultScale;
        }

        public void Enable(bool state)
        {
            _state = state;
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            _isPointerDown = false;
        
            SetScale(false);
            SetCanvasRenderersColor(colors.normalColor);
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            _isPointerInside = true;
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            _isPointerInside = false;
        
            SetScale(false);
            SetCanvasRenderersColor(colors.normalColor);
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            _isPointerDown = true;
        
            SetScale(true);
            SetCanvasRenderersColor(colors.pressedColor);
        }
        
        private void SetCanvasRenderersColor(Color color)
        {
            if (!_state || !IsInteractable()) 
                return;

            foreach (var childRenderer in _renderers)
            {
                if (!_ignoreRenderers.Contains(childRenderer))
                {
                    childRenderer.SetColor(color);
                }
            }
        }

        private void SetScale(bool isDown)
        {
            if (!_haveBounceAnimation || (!IsInteractable() && isDown)) 
                return;
        
            if (!DOTween.IsTweening(Id))
            {
                Vector3 targetScale = _defaultScale * (isDown ? _targetScaleFactor : 1f);
                
                if (targetScale.AlmostEquals(transform.localScale, 0.001f)) 
                    return;
                
                transform.DOScale(targetScale, _animationTime)
                    .SetId(Id)
                    .SetUpdate(true)
                    .SetEase(Ease.OutFlash)
                    .OnComplete(() =>
                    {
                        if (isDown && (!_isPointerInside || !_isPointerDown)) 
                            SetScale(false);
                    });
            }
        }

        private void RefreshReferences()
        {
            _renderers = GetComponentsInChildren<CanvasRenderer>(true);
        }

        protected override void OnCanvasHierarchyChanged()
        {
            base.OnCanvasHierarchyChanged();

            RefreshReferences();
        }

#if UNITY_EDITOR

        protected override void OnValidate()
        {
            base.OnValidate();

            RefreshReferences();
        }

        protected override void Reset()
        {
            base.Reset();
            _ignoreRenderers = Array.Empty<CanvasRenderer>();
        }

#endif
    }

}