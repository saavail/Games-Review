using System;
using DependencyInjector;
using DG.Tweening;
using Graphics;
using UnityEngine;
using UnityEngine.UI;

namespace Games.View
{
    public class Cell : MonoBehaviour
    {
        [SerializeField]
        protected Image _background;
        [SerializeField]
        protected Image _numberImage;

        public int X { get; private set; }
        public int Y { get; private set; }

        protected virtual void Setup(int value, Color color)
        {
            _numberImage.sprite = AllServices.Container.Single<IIconsService>().GetIcon(value.ToString());
            _background.color = color;
        }

        public void Push(int x, int y, int value, Color color)
        {
            X = x;
            Y = y;
            
            Setup(value, color);
            
            transform.DOKill();

            transform.DOScale(Vector3.one, 0.1f)
                .SetDelay(0.1f)
                .SetEase(Ease.OutBack)
                .From(Vector3.zero);
        }

        public void Merge(int value, Color color)
        {
            Setup(value, color);

            transform.DOScale(Vector3.one * 1.2f, 0.125f)
                .SetLoops(2, LoopType.Yoyo)
                .From(Vector3.one);
        }

        public void Move(int x, int y, Vector2 position)
        {
            X = x;
            Y = y;
            
            transform.DOKill();

            ((RectTransform) transform).DOAnchorPos(position, 0.15f);
        }

        public void Remove(Vector2 position, Action<Cell> callback = null)
        {
            transform.DOKill();
            
            ((RectTransform) transform).DOAnchorPos(position, 0.15f)
                .OnComplete(() => callback?.Invoke(this));
            
            transform.DOScale(Vector3.zero, 0.05f)
                .From(Vector3.one)
                .SetDelay(0.1f);
        }

        public void Delete(Action callback)
        {
            transform.DOKill();

            transform.DOScale(Vector3.zero, 0.25f)
                .From(Vector3.one)
                .OnComplete(() => callback?.Invoke());
        }
    }
}