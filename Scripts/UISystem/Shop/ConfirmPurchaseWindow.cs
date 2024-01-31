using System.Linq;
using Core.UserStuff;
using DependencyInjector;
using Enums;
using Graphics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UISystem.Shop
{
    public class ConfirmPurchaseWindow : Window
    {
        [Header("Slot")]
        [SerializeField]
        private Image _slotImage;
        [SerializeField]
        private TextMeshProUGUI _slotName;
        [SerializeField]
        private TextMeshProUGUI _slotDescription;
        [SerializeField]
        private TextMeshProUGUI _slotPrice;

        [Header("Currencies")]
        [SerializeField]
        private CurrencyScroll _currenciesScroll;
        [SerializeField]
        private TextMeshProUGUI _balanceTitle;
        [SerializeField]
        private TextMeshProUGUI _balanceText;

        [Header("Order Summary")]
        [SerializeField]
        private TextMeshProUGUI _summaryTitle;
        [SerializeField]
        private TextMeshProUGUI _summaryName;
        [SerializeField]
        private TextMeshProUGUI _summaryValue;
        [SerializeField]
        private TextMeshProUGUI _commissionName;
        [SerializeField]
        private TextMeshProUGUI _commissionValue;
        [SerializeField]
        private TextMeshProUGUI _totalName;
        [SerializeField]
        private TextMeshProUGUI _totalValue;
        [SerializeField]
        private Button _payButton;
        
        private SlotData _data;

        private void Start()
        {
            _balanceTitle.text = "Balance:";
            _summaryTitle.text = "Order Summary";
            _summaryName.text = "Summary";
            _commissionName.text = "Commission";
            _totalName.text = "Total";
            
            _payButton.onClick.AddListener(OnPayButtonClick);
        }

        // private void OnEnable()
        // {
        //     _currenciesScroll.OnSelectionChanged += CurrencyScroll_SelectionChanged;
        // }
        //
        // private void OnDisable()
        // {
        //     _currenciesScroll.OnSelectionChanged -= CurrencyScroll_SelectionChanged;
        // }

        protected override void OnOpen() { }

        public override void Refresh()
        {
            // if (_data == default || _response == default)
            //     return;
            //
            // _currenciesScroll.UpdateData(_response.Currencies);
            // _currenciesScroll.SelectCell(_response.Currencies.ToList().IndexOf(_response.Currency));
            //
            // SetupSlot(_data);
            // SetupOrderSummary(_data);
            //
            // var user = AllServices.Container.Single<IUserService>().User;
            //
            // _balanceText.text = $"{user.GetMoney(_data.PriceType)} {_data.PriceType} / {user.GetMoney(_data.PriceType)} {_data.PriceType}";

        }

        protected override void OnClose()
        {
            AllServices.Container.Single<WindowSystem>().UserProfile.OpenAll();
        }

        protected override void Localize() { }

        // public void Setup(ShopSlotData data, SlotResponse response)
        // {
        //     _data = data;
        //     _response = response;
        //
        //     Refresh();
        // }

        // private void SetupSlot(ShopSlotData data)
        // {
        //     var iconsService = AllServices.Container.Single<IIconsService>();
        //
        //     if (data.GameReference != GameType.None)
        //     {
        //         _slotImage.sprite = iconsService.GetIcon(data.GameReference);
        //         
        //         _slotName.text = data.GameReference.ToString();
        //         _slotDescription.text = data.GameReference.ToString();
        //     }
        //     else
        //     {
        //         _slotImage.sprite = iconsService.GetShopPreview(data.SlotId);
        //         
        //         _slotName.text = data.RewardType.ToString();
        //         _slotDescription.text = data.RewardType.ToString();
        //     }
        //
        //     _slotPrice.text = $"{data.Price} {data.PriceType}";
        // }

        // private void SetupOrderSummary(ShopSlotData data)
        // {
        //     _summaryValue.text = $"{data.Price} {data.PriceType}";
        //     _commissionValue.text = $"-{0} {data.PriceType}";
        //     _totalValue.text = $"{data.Price} {data.PriceType}";
        // }
        // private void CurrencyScroll_SelectionChanged(int index)
        // {
        //     _selectedCrypto = _response.Currencies[index];
        // }

        private void OnPayButtonClick()
        {
            // AllServices.Container.Single<IUserService>().BuyHardSlot(_data, _selectedCrypto, (isSuccess, errorCode) =>
            // {
            //     Close();
            //
            //     var messagePopup = AllServices.Container.Single<WindowSystem>().Show<MessagePopup>();
            //     
            //     if (isSuccess)
            //     {
            //         messagePopup.Setup(AllServices.Container.Single<IIconsService>().GetMoneyMessageIcon, "You purchased item", "Ok", null);
            //         AllServices.Container.Single<WindowSystem>().UserProfile.UpdateWalletInfo();
            //     }
            //     else
            //     {
            //         messagePopup.SetupError(errorCode);
            //     }
            // });
        }
    }
}