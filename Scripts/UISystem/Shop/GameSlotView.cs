using Core.UserStuff;
using DependencyInjector;
using Graphics;
using Localization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UISystem.Shop
{
    public class GameSlotView : ShopSlotBase
    {
        [SerializeField]
        private TextMeshProUGUI _title;
        [SerializeField]
        private GameObject _boughtGroup;
        [SerializeField]
        private TextMeshProUGUI _boughtText;
        [SerializeField]
        private Image _selectedImage;

        public override void Start()
        {
            base.Start();
            _boughtText.text = "BOUGHT";
        }

        public override void Setup(SlotData data)
        {
            base.Setup(data);
            
            // _title.text = data.LocalizationSlotId.Localized();

            var iconsService = AllServices.Container.Single<IIconsService>();

            _mainImage.sprite = iconsService.GetShopPreview(data.slotId);
            _mainImage.SetNativeSize();

            var userService = AllServices.Container.Single<IUserService>();

            bool isPurchased = userService.User.IsPurchased(data.slotId);
            _priceField.gameObject.SetActive(!isPurchased);
            _boughtGroup.SetActive(isPurchased);

            _selectedImage.enabled = userService.User.GetChooseSkin(Data.gameType, Data.skinType) == Data.skinCollectionType;
            _boughtText.text = userService.User.GetChooseSkin(Data.gameType, Data.skinType) == Data.skinCollectionType ? "CHOSEN" : "BOUGHT";
        }
    }
}