using System;
using System.Globalization;
using Core.UserStuff;
using DependencyInjector;
using DG.Tweening;
using Enums;
using TMPro;
using UISystem.Shop;
using UnityEngine;
using UnityEngine.UI;

namespace UISystem
{
    public class UserProfile : MonoBehaviour
    {
        private readonly string _guestName = "Guest";
        
        private const float YWalletScaleFull = 609f;
        private const float YWalletScaleCompacted = 367f;
        private const float YWalletPositionFull = 68f;
        private const float YWalletPositionCompacted = 189f;
        private const float AnimationDuration = 0.25f;
        private const float YUserProfileFull = 0f;
        private const float YUserProfileCompacted = 340f;
        
        [SerializeField]
        private RectTransform _walletWindow;
        [SerializeField]
        private RectTransform _userWindow;
        
        [SerializeField] 
        private TextMeshProUGUI _ticketWallet;
        [SerializeField] 
        private TextMeshProUGUI _gemWallet;
        [SerializeField] 
        private TextMeshProUGUI _goldWallet;

        [SerializeField]
        private Button _exitButton;

        [SerializeField]
        private Image _userAvatar;
        [SerializeField]
        private TextMeshProUGUI _userName;

        [SerializeField]
        private Image _profileProgressSlider;
        [SerializeField]
        private Image _profileProgressSliderBack;
        [SerializeField]
        private TextMeshProUGUI _levelText;

        private IUserService _userService;
        private WindowSystem _windowSystem;

        private int _lastUserLevel;
        
        public void Initialize(IUserService userService)
        {
            _userService = userService;

            _lastUserLevel = _userService.User.Level;
            
            SetUserInfo();
            
            _windowSystem = AllServices.Container.Single<WindowSystem>();
            _exitButton.onClick.AddListener(GoToGubWindowButton);
            UpdateWalletInfo();
            
            PlayLevelAnimation();
        }

        private void SetUserInfo()
        {
            _userName.text = _guestName;
            _levelText.text = $"LVL {_userService.User.Level}";

            _profileProgressSlider.fillAmount = 0f;
            _profileProgressSliderBack.fillAmount = 0f;
        }

        public void UpdateWalletInfo()
        {
            _ticketWallet.text = _userService.User.GetMoney(CurrencyType.Ticket).ToString(CultureInfo.InvariantCulture);
            _gemWallet.text = _userService.User.GetMoney(CurrencyType.Gems).ToString(CultureInfo.InvariantCulture);
            _goldWallet.text = _userService.User.GetMoney(CurrencyType.Gold).ToString(CultureInfo.InvariantCulture);
        }

        public void SetFullView()
        {
            SetActiveByView(UserProfileScreenType.Full);
            SetWalletAnimation(UserProfileScreenType.Full);
        }
        
        public void SetCompactView()
        {
            SetActiveByView(UserProfileScreenType.Compact);
            SetWalletAnimation(UserProfileScreenType.Compact);
        }
        
        public void CloseAll()
        {
            _userWindow.gameObject.SetActive(false);
            _walletWindow.gameObject.SetActive(false);

            _profileProgressSlider.DOKill(true);
            _levelText.DOKill(true);
        }

        public void OpenAll()
        {
            _userWindow.gameObject.SetActive(true);
            _walletWindow.gameObject.SetActive(true);
            
            PlayLevelAnimation();
        }

        private void PlayLevelAnimation()
        {
            const float baseDuration = 0.7f;

            int levels = _userService.User.Level - _lastUserLevel;
            int animatedLevels = Mathf.Min(5, levels);
            var sequence = DOTween.Sequence(_profileProgressSlider)
                .SetDelay(0.2f);

            for (int i = 0; i < animatedLevels; i++)
            {
                float duration;
                Ease ease;

                if (i == 0)
                {
                    duration = (1f - _profileProgressSlider.fillAmount) * baseDuration;
                    ease = Ease.InSine;
                }
                else
                {
                    duration = Mathf.Max(baseDuration * Mathf.Pow(0.9f, i), 0.25f);
                    ease = Ease.Linear;
                }

                var level = _userService.User.Level - (levels / animatedLevels * (animatedLevels - (i + 1)));

                AddLevelAnimation(sequence, 1f, duration, ease, level, i > 0);
            }

            float levelPercent = _userService.GetLevelPercent();
            AddLevelAnimation(sequence, levelPercent, levelPercent * baseDuration, Ease.OutSine, -1, animatedLevels > 0);

            _lastUserLevel = _userService.User.Level;
        }

        private void AddLevelAnimation(Sequence sequence, float to, float duration, Ease ease, int level, bool fromZero)
        {
            sequence.Append(_profileProgressSlider.DOFillAmount(to, duration)
                .SetEase(ease)
                .OnStart(() =>
                {
                    if (fromZero)
                    {
                        _profileProgressSliderBack.fillAmount = 0f;
                    }
                    
                    _profileProgressSliderBack.DOKill();
                    
                    _profileProgressSliderBack.DOFillAmount(to, duration)
                        .SetEase(ease);
                })
                .OnComplete(() =>
                {
                    if (level > 0)
                    {
                        _levelText.text = $"LVL {level}";
                        _levelText.DOKill();
                        
                        _levelText.transform.DOScale(Vector3.one * 1.3f, Mathf.Max(duration / 2f, 0.1f))
                            .SetLoops(2, LoopType.Yoyo)
                            .SetEase(Ease.InOutSine)
                            .From(Vector3.one);                        
                    }

                    if (to >= 1f)
                    {
                        _profileProgressSlider.fillAmount = 0f;
                        _profileProgressSliderBack.fillAmount = 0f;
                    }
                }));
        }

        private void SetActiveByView(UserProfileScreenType screenType)
        {
            _exitButton.gameObject.SetActive(screenType != UserProfileScreenType.Full);
        }

        private void SetWalletAnimation(UserProfileScreenType screenType)
        {
            if (screenType == UserProfileScreenType.Full) 
            {
                AnimateWalletWindow(YWalletScaleFull, YWalletPositionFull, Ease.InBack);
                AnimateUserWindow(YUserProfileFull, Ease.InBack);
            } 
            else 
            {
                AnimateWalletWindow(YWalletScaleCompacted, YWalletPositionCompacted, Ease.OutBack);
                AnimateUserWindow(YUserProfileCompacted, Ease.OutBack);
            }
        }

        private void AnimateUserWindow(float to, Ease easeType)
        {
            _userWindow.DOLocalMoveY(to, AnimationDuration)
                .SetEase(easeType);
        }

        private void AnimateWalletWindow(float yScale, float yPosition, Ease easeType) 
        {
            _walletWindow.DOSizeDelta(new Vector2(_walletWindow.sizeDelta.x, yScale), AnimationDuration)
                .SetEase(easeType);
            _walletWindow.DOLocalMoveY(yPosition, AnimationDuration)
                .SetEase(easeType);;
        }

        private void GoToGubWindowButton()
        {
            if (_windowSystem.Current.GetType() == typeof(ShopWindow))
            {
                _windowSystem.CloseCurrentWindow(SetFullView);
            }
        }
    }
}