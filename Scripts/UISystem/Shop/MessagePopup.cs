using System;
using DependencyInjector;
using Graphics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UISystem.Shop
{
    public class MessagePopup : Window
    {
        [SerializeField]
        private TextMeshProUGUI _description;
        [SerializeField]
        private Button _actionButton;
        [SerializeField]
        private TextMeshProUGUI _buttonText;
        [SerializeField]
        private Image _mainImage;

        private Action _onButtonClick;
        
        private void Start()
        {
            _actionButton.onClick.AddListener(OnActionButtonClick);
        }

        public void Setup(Sprite sprite, string description, string buttonText, Action onButtonClick)
        {
            SetActiveSprite(sprite);
            
            _description.text = description;
            _buttonText.text = buttonText;
            _onButtonClick = onButtonClick;
        }

        public void SetupError(int errorCode)
        {
            Setup(AllServices.Container.Single<IIconsService>().GetGameIcon,
                $"Error {errorCode}: connection error, try again", Strings.CommonOk, null);
        }
        
        protected override void OnOpen() { }
        public override void Refresh() { }
        protected override void OnClose() { }
        protected override void Localize() { }

        private void OnActionButtonClick()
        {
            Close();
            _onButtonClick?.Invoke();
        }

        private void SetActiveSprite(Sprite sprite)
        {
            if (sprite == null)
            {
                _mainImage.enabled = false;
            }
            else
            {
                _mainImage.enabled = true;
                _mainImage.sprite = sprite;
                _mainImage.SetNativeSize();
            }
        }

        private void OnDisable()
        {
            Destroy(gameObject);    
        }
    }
}