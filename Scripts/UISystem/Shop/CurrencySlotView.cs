using Core.UserStuff;
using DependencyInjector;
using Graphics;
using UnityEngine;

namespace UISystem.Shop
{
    public class CurrencySlotView : ShopSlotBase
    {
        [SerializeField]
        private CurrencyField _rewardField;

        public override void Setup(SlotData data)
        {
            base.Setup(data);

            var iconsService = AllServices.Container.Single<IIconsService>();

            _rewardField.Setup(data.reward.currencyType, data.reward.amount, "x");

            _mainImage.sprite = iconsService.GetShopPreview(data.slotId);
            _mainImage.SetNativeSize();

            switch (data.slotId)
            {
                case 4:
                    _mainImage.transform.localScale = Vector3.one * 0.9f;
                    break;

                case 5:
                    _mainImage.transform.localScale = Vector3.one * 0.8f;
                    break;

                case 6:
                    _mainImage.transform.localScale = Vector3.one * 0.7f;
                    break;

                default:
                    _mainImage.transform.localScale = Vector3.one;
                    break;
            }
        }
    }
}