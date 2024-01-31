using System;
using System.Globalization;
using System.Threading;
using Core;
using Cysharp.Threading.Tasks;
using DependencyInjector;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UISystem
{
    public class RevivalWindow : Window
    {
        [SerializeField]
        private Button _revivalButton;
        [SerializeField]
        private Image _currencyTypeImage;
        [SerializeField]
        private TextMeshProUGUI _revivalCostText;

        [SerializeField]
        private int _revivalTime;
        [SerializeField]
        private Image _timerImage;
        [SerializeField]
        private TextMeshProUGUI _timerText;

        [SerializeField]
        private TextMeshProUGUI _losePopUp;
        [SerializeField]
        private TextMeshProUGUI _restoreProgressPopUp;

        public event Action OnReviveClick;
        public event Action OnTimeEnded;

        private float _revivalCost;
        private Sequence _animSequence;
        private CancellationTokenSource _cancellationTokenSource;

        protected override void OnOpen()
        {
            _timerText.text = _revivalTime.ToString();
            _revivalButton.onClick.AddListener(ReviveButton);
            StartTimer();
        }

        public void SetRevivalCost(float revivalCost, Sprite revivalImage)
        {
            _revivalCost = revivalCost;
            _revivalCostText.text = _revivalCost.ToString(CultureInfo.InvariantCulture);
            _currencyTypeImage.sprite = revivalImage;
        }

        public override void Refresh() { }

        protected override void OnClose()
        {
            _timerImage.fillAmount = 1f;
            _revivalButton.onClick.RemoveAllListeners();

            var game = AllServices.Container.Single<GamesService>().Factory.Current;
            
            if (game.UseRevive)
                return;
            
            int score = game.Score;
            int maxScore = game.GetMaxScore();
            int coins = game.CalculateCoins();

            AllServices.Container.Single<WindowSystem>()
                .Show<RewardWindow>()
                .SetWindowValues(score, maxScore, coins);
        }

        protected override void Localize()
        {
            _losePopUp.text = Strings.YouLose + " ! ";
            _restoreProgressPopUp.text = Strings.RestoreProgress + " ? ";
        }

        private void StartTimer()
        {
            TimerAnimation();
            _cancellationTokenSource = new CancellationTokenSource();
            Countdown(_revivalTime,_cancellationTokenSource.Token).Forget();
        }
        
        private async UniTask Countdown(int counter, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            while (counter > 0)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(1f), cancellationToken: cancellationToken);
                counter--;
                _timerText.text = counter.ToString();
            }
        }
        
        private void TimerAnimation()
        {
            _animSequence = DOTween.Sequence();
            
            _animSequence.Append(_timerImage.DOFillAmount(0f, _revivalTime)
                .SetEase(Ease.Linear))
                .OnComplete(() => OnTimeEnded?.Invoke());
        }

        private void ReviveButton()
        {
            _animSequence.Kill();
            
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();
                _cancellationTokenSource = null;
            }
            
            OnReviveClick?.Invoke();
        }
    }
}