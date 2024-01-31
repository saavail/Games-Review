using System.Collections.Generic;
using Core.UserStuff;
using DependencyInjector;
using Enums;
using Graphics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace UISystem.Shop
{
    public class ShopGroup : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _titleText;
        [SerializeField]
        private Image _titleIcon;
        [SerializeField]
        private RectTransform _slotsRoot;
        [SerializeField]
        private ShopSlotBase _slotPrefab;

        private readonly List<ShopSlotBase> _slots = new();
        private SlotData[] _data;

        public void Setup(SlotData[] data)
        {
            _data = data;
            
            MonoBehaviourUtility.SpawnOrRefreshList(_slots, data.Length, _slotsRoot, _slotPrefab, (view, index) =>
            {
                view.Setup(data[index]);
            });

            var gameType = data[0].gameType;
            
            if (gameType != GameType.None)
            {
                _titleIcon.gameObject.SetActive(true);

                _titleIcon.sprite = AllServices.Container.Single<IIconsService>().GetIcon(gameType);
                _titleIcon.SetNativeSize();
            }
            else
            {
                _titleText.text = data[0].reward.currencyType.ToString();
                _titleIcon.gameObject.SetActive(false);
            }
            
            Localize(data);
        }

        private void Localize(SlotData[] data)
        {
            var gameType = data[0].gameType;
            
            switch (gameType)
            {
                case GameType.Game2048:
                    _titleText.text = Strings.Game2048;
                    break;
                
                case GameType.FlappyPlane:
                    _titleText.text = Strings.GameFlappyPlane;
                    break;
                
                case GameType.RoosterRunner:
                    _titleText.text = Strings.GameRoosterRunner;
                    break;
                
                case GameType.None:
                    
                    switch (data[0].reward.currencyType)
                    {
                        case CurrencyType.Gems:
                            _titleText.text = Strings.Gems;
                            break;
                        
                        case CurrencyType.Gold:
                            _titleText.text = Strings.Gold;
                            break;
                        
                        case CurrencyType.Ticket:
                            _titleText.text = Strings.Ticket;
                            break;
                        
                        case CurrencyType.USD:
                            _titleText.text = data[0].reward.currencyType.ToString();
                            break;
                        
                        case CurrencyType.None:
                            _titleText.text = data[0].reward.currencyType.ToString();
                            break;
                    }
                    
                    break;
            }
        }
    }
}