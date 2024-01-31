using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UISystem.Shop
{
    public class NoInternetPopup : Window
    {
        [SerializeField]
        private TextMeshProUGUI _description;
        [SerializeField]
        private Image _mainImage;
        [SerializeField]
        private Button _actionButton1;
        [SerializeField]
        private TextMeshProUGUI _buttonText1;
        [SerializeField]
        private Button _actionButton2;
        [SerializeField]
        private TextMeshProUGUI _buttonText2;
        [SerializeField]
        private Button _actionButton3;
        [SerializeField]
        private TextMeshProUGUI _buttonText3;

        private Action _action2;
        private Action _action3;

        protected override void Awake()
        {
            base.Awake();
            
            _actionButton1.onClick.AddListener(OnFirstButtonClick);
            _actionButton2.onClick.AddListener(OnSecondButtonClick);
            _actionButton3.onClick.AddListener(OnThirdButtonClick);

            _buttonText1.text = "Skip";
            _buttonText2.text = "Guest";
            _buttonText3.text = "Try again";
        }

        protected override void Localize()
        {
            _description.text = "Something went wrong, check your internet connection!";
        }

        [Button]
        protected override void OnOpen()
        {
            ShowButtonAnimation(_actionButton1, 0.2f);
            ShowButtonAnimation(_actionButton2, 0.4f);
            ShowButtonAnimation(_actionButton3, 0.6f);
        }

        public override void Refresh()
        {
        }

        protected override void OnClose()
        {
            _actionButton1.DOKill();
            _actionButton2.DOKill();
            _actionButton3.DOKill();
        }

        private void ShowButtonAnimation(Button button, float delay)
        {
            button.transform.DOScale(Vector3.one, 0.3f)
                .From(Vector3.zero)
                .SetEase(Ease.OutBack)
                .SetDelay(delay);
        }

        public void SetupActions(Action guestAction, Action tryAgainAction)
        {
            _action2 = guestAction;
            _action3 = tryAgainAction;
        }

        private void OnFirstButtonClick()
        {
            Close();
        }

        private void OnSecondButtonClick()
        {
            _action2?.Invoke();
        }

        private void OnThirdButtonClick()
        {
            _action3?.Invoke();
        }
    }
}