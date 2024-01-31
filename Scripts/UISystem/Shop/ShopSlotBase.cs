using System;
using Core.UserStuff;
using DependencyInjector;
using DG.Tweening;
using Graphics;
using UnityEngine;
using UnityEngine.UI;

namespace UISystem.Shop
{
    public abstract class ShopSlotBase : MonoBehaviour
    {
        [Header("Animation")]
        [SerializeField]
        private Image _waitGroup;
        [SerializeField]
        private Image _waitImage;
        
        [Header("Main")]
        [SerializeField]
        protected Image _background;
        [SerializeField]
        protected Image _mainImage;
        [SerializeField]
        protected CurrencyField _priceField;
        [SerializeField]
        private Button _mainButton;

        public SlotData Data { get; private set; }

        public static event Action<SlotData, Action> OnSlotClick;

        public virtual void Start()
        {
            _mainButton.onClick.AddListener(OnSlotButtonClick);
        }

        public void SetupYourSelf()
        {
            Setup(Data);
        }

        public virtual void Setup(SlotData data)
        {
            Data = data;
            _priceField.Setup(data.price.currencyType, data.price.amount);
        }

        private void OnSlotButtonClick()
        {
            var user = AllServices.Container.Single<IUserService>().User;

            if (!user.IsPurchased(Data.slotId) && !user.CanBuy(Data.price.currencyType, Data.price.amount))
            {
                AllServices.Container.Single<WindowSystem>()
                    .Show<MessagePopup>()
                    .Setup(AllServices.Container.Single<IIconsService>().GetMoneyMessageIcon, 
                        "Not enough money", Strings.CommonOk, null);
                
                return;
            }

            ShowWaitAnimation();
            
            OnSlotClick?.Invoke(Data, () =>
            {
                HideWaitAnimation();
                SetupYourSelf();
            });
        }

        public void ShowWaitAnimation()
        {
            _waitGroup.gameObject.SetActive(true);

            _waitGroup.DOKill();
            _waitImage.DOKill();

            DOTween.Sequence(_waitImage)
                .Append(_waitImage.transform.DOScale(Vector3.one, 0.3f)
                    .From(Vector3.zero)
                    .SetEase(Ease.OutBack))
                .Join(_waitImage.DOFade(0.65f, 0.3f)
                    .From(0f)
                    .SetEase(Ease.OutSine))
                .Append(_waitImage.transform.DOScale(Vector3.one * 0.75f, 0.25f)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetEase(Ease.InOutSine));

            _waitGroup.DOFade(1f, 0.3f)
                .From(0f)
                .SetEase(Ease.OutSine);
        }

        public void HideWaitAnimation()
        {
            _waitGroup.DOKill();
            _waitImage.DOKill();

            DOTween.Sequence(_waitImage)
                .Append(_waitImage.transform.DOScale(Vector3.zero, 0.3f)
                    .SetEase(Ease.InSine))
                .Join(_waitImage.DOFade(0f, 0.3f)
                    .SetEase(Ease.InSine));
            
            _waitGroup.DOFade(0f, 0.3f)
                .From(1f)
                .SetEase(Ease.InSine)
                .OnComplete(() => _waitGroup.gameObject.SetActive(false));
        }
    }
}