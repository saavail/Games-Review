using System;
using DependencyInjector;
using TMPro;
using UISystem;
using UISystem.Shop;
using UnityEngine;
using UnityEngine.UI;

namespace Hub
{
    public class HubWindow : Window
    {
        [SerializeField]
        private Button _shopButton;
        [SerializeField]
        private Button _activitiesButton;
        [SerializeField]
        private TextMeshProUGUI _shopButtonText;
        [SerializeField]
        private TextMeshProUGUI _activitiesButtonText;
        [SerializeField]
        private TextMeshProUGUI _popularGames;

        protected override bool CanAnimate => false;

        protected override void Awake()
        {
            base.Awake();

            _shopButton.onClick.AddListener(OnShopClick);
            _activitiesButton.onClick.AddListener(OnActivitiesClick);
        }

        protected override void Localize()
        {
            _popularGames.text = Strings.PopularGames;
            _shopButtonText.text = Strings.CommonShop;
            _activitiesButtonText.text = Strings.CommonActivities;
        }

        private void OnShopClick()
        {
            var windowSystem = AllServices.Container.Single<WindowSystem>();
            windowSystem.Show<ShopWindow>(() => windowSystem.UserProfile.SetCompactView());
        }

        private void OnActivitiesClick()
        {
            var windowSystem = AllServices.Container.Single<WindowSystem>();
            
            windowSystem
                .Show<MessagePopup>()
                .Setup(null, "Activities", Strings.CommonOk, null);
        }

        protected override void OnOpen() { }

        public override void Refresh() { }

        protected override void OnClose() { }
    }
}