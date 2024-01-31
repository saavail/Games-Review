using Core;
using DependencyInjector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UISystem
{
    public class RewardWindow : Window
    {
        [SerializeField]
        private Button _retryGameButton;
        [SerializeField]
        private Button _watchAdsButton;
        [SerializeField]
        private Button _exitGameButton;

        [SerializeField]
        private TextMeshProUGUI _rewardValue;
        [SerializeField]
        private TextMeshProUGUI _currentScore;
        [SerializeField]
        private TextMeshProUGUI _bestScore;

        [SerializeField]
        private TextMeshProUGUI _scoreTitleText;
        [SerializeField]
        private TextMeshProUGUI _bestScoreTitleText;
        [SerializeField]
        private TextMeshProUGUI _rewardTitleText;
        [SerializeField]
        private TextMeshProUGUI _watchAdsText;
        [SerializeField]
        private TextMeshProUGUI _doubleRewardText;

        private void Start()
        {
            _retryGameButton.onClick.AddListener(RetryGame);
            _watchAdsButton.onClick.AddListener(WatchAds);
            _exitGameButton.onClick.AddListener(ExitGame);
        }

        public void SetWindowValues(int currentScore, int bestScore, int rewardValue)
        {
            _currentScore.text = currentScore.ToString();
            _bestScore.text = bestScore.ToString();
            _rewardValue.text = rewardValue.ToString();
        }

        protected override void OnOpen() { }
        public override void Refresh()
        {
            
        }

        protected override void OnClose() { }

        protected override void Localize()
        {
            _scoreTitleText.text = Strings.CommonScore + ":";
            _bestScoreTitleText.text = Strings.CommonBestScore + ":";
            _rewardTitleText.text = Strings.CommonReward;
            _watchAdsText.text = Strings.WatchAds;
            _doubleRewardText.text = Strings.DoubleRewards;
        }

        private void RetryGame()
        {
            AllServices.Container.Single<GamesService>().Factory.Current.Restart();
        }

        private void WatchAds()
        {
            
        }
        
        private void ExitGame()
        {
            AllServices.Container.Single<GamesService>().Factory.Current.ExitAsync();
        }
    }
}