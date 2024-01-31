using DependencyInjector;
using Enums;
using Graphics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace UISystem.Shop
{
    public class CurrencyCell : FancyCell<CryptoType, Context>
    {
        static class AnimatorHash
        {
            public static readonly int Scroll = Animator.StringToHash("scroll");
        }
        
        [SerializeField]
        private Image _mainImage;
        [SerializeField]
        private TextMeshProUGUI _nameText;
        [SerializeField]
        private Button _mainButton;
        [SerializeField]
        private GameObject _selectGroup;
        [SerializeField]
        private Animator _animator;
        
        private float _currentPosition = 0;

        private void Start()
        {
            _mainButton.onClick.AddListener(OnMainClick);
        }

        private void OnEnable()
        {
            UpdatePosition(_currentPosition);
        }

        public override void UpdateContent(CryptoType cryptoType)
        {
            var iconsService = AllServices.Container.Single<IIconsService>();
            
            _selectGroup.SetActive(Context.SelectedIndex == Index);

            _mainImage.sprite = iconsService.GetIcon(cryptoType);
            _nameText.text = cryptoType.ToString();
        }

        public override void UpdatePosition(float position)
        {
            _currentPosition = position;

            if (_animator.isActiveAndEnabled)
            {
                _animator.Play(AnimatorHash.Scroll, -1, position);
            }

            _animator.speed = 0;
        }
        
        private void OnMainClick()
        {
            Context.OnCellClicked?.Invoke(Index);
        }
    }
}