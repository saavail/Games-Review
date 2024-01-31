using Core;
using DependencyInjector;
using DG.Tweening;
using Games;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UISystem
{
    public class ScoreWindow : Window
    {
        [SerializeField]
        private Button _retryGameButton;
        [SerializeField]
        private Button _exitGameButton;

        [SerializeField]
        private TextMeshProUGUI _currentScoreText;
        [SerializeField]
        private TextMeshProUGUI _coinsScoreText;
        [SerializeField]
        private TextMeshProUGUI _bestScoreText;
        
        protected override bool CanAnimateContent => false;
        
        private void Start()
        {
            _retryGameButton.onClick.AddListener(RetryGame);
            _exitGameButton.onClick.AddListener(ExitGame);
        }

        public void SetWindowValues(int currentScore, int coins, int bestScore)
        {
            _currentScoreText.text = currentScore.ToString();
            _coinsScoreText.text = coins.ToString();
            _bestScoreText.text = bestScore.ToString();
        }

        public void SetMaxScore(int bestScore)
        {
            _bestScoreText.text = bestScore.ToString();
        }
        
        protected override void OnOpen() { }
        public override void Refresh() { }

        protected override void OnClose()
        {
            ResetValues();
            SetActiveRestartButton(true);
        }

        protected override void Localize() { }

        public void ResetValues()
        {
            _currentScoreText.text = "0";
            _coinsScoreText.text = "0";
            _bestScoreText.text = "0";
        }

        public void SetActiveRestartButton(bool state)
        {
            _retryGameButton.gameObject.SetActive(state);
            _exitGameButton.gameObject.SetActive(state);
        }
        
        private void RetryGame()
        {
            if (AllServices.Container.Single<GamesService>().Factory.Current.GetType() != typeof(Game2048))
            {
                AllServices.Container.Single<GamesService>().Factory.Current.FastRestart();
            }
        }

        private void ExitGame()
        {
            AllServices.Container.Single<GamesService>().Factory.Current.ExitAsync();
        }
    }
}