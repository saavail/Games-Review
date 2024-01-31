using Core;
using DependencyInjector;
using Enums;
using Graphics;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace UISystem.Shop
{
    public class CurrencyField : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _priceText;
        [SerializeField]
        private Image _currencyImage;
        [SerializeField]
        private Image _backgroundImage;

        public void Setup(CurrencyType currencyType, float price, string prefix = "")
        {
            var iconsService = AllServices.Container.Single<IIconsService>();

            if (price <= 0)
            {
                _priceText.text = "Free";
                _currencyImage.gameObject.SetActive(false);
            }
            else if (currencyType is CurrencyType.USD or CurrencyType.USDTest)
            {
                _priceText.text = $"${price}";
                _currencyImage.gameObject.SetActive(false);
            }
            else
            {
                _priceText.text = $"{prefix}{Mathf.RoundToInt(price)}";
                _currencyImage.gameObject.SetActive(true);
                
                _currencyImage.sprite = iconsService.GetIcon(currencyType);
                _currencyImage.SetNativeSize();
            }
        }

        public void SetTextColor(Color color)
        {
            _priceText.color = color;
        }
    }
}