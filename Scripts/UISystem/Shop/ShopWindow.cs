using System.Collections.Generic;
using System.Linq;
using DependencyInjector;
using EntryPoint;
using Enums;
using UnityEngine;
using UnityEngine.UI;

namespace UISystem.Shop
{
    public class ShopWindow : Window
    {
        [SerializeField] 
        private ScrollRect _scrollRect;
        [SerializeField]
        private ShopGroup _currencyGroupPrefab;
        [SerializeField]
        private ShopGroup _gameGroupPrefab;
        [SerializeField]
        private RectTransform _groupsRoot;

        private readonly List<ShopGroup> _groups = new();
        private ShopSlotBase _openedSlot;

        protected override bool CanAnimate => false;

        protected override void OnOpen() { }

        public override void Refresh()
        {
            var scrollPosition = _scrollRect.verticalNormalizedPosition;

            var slots = AllServices.Container.Single<IBalanceService>().RemoteBalance.slotsData;

            var groups = slots.ToList().GroupBy(i => i.gameType).ToArray();
            
            int index = 0;
            for (; index < groups.Length && index < _groups.Count; index++)
            {
                _groups[index].gameObject.SetActive(true);
                _groups[index].Setup(groups[index].ToArray());
            }
            
            for (; index < groups.Length; index++)
            {
                var data = groups[index].ToArray();

                ShopGroup prefab = data[0].gameType != GameType.None ? _gameGroupPrefab : _currencyGroupPrefab;
                
                ShopGroup behaviour = Instantiate(prefab, _groupsRoot);
                behaviour.Setup(data);
                _groups.Add(behaviour);
            }

            for (; index < _groups.Count; index++)
            {
                _groups[index].gameObject.SetActive(false);
            }
            
            SetScrollPosition(scrollPosition);
        }

        private void SetScrollPosition(float scrollPosition)
        {
            Canvas.ForceUpdateCanvases();
            
            _scrollRect.content.GetComponent<VerticalLayoutGroup>().CalculateLayoutInputVertical() ;
            _scrollRect.verticalNormalizedPosition = scrollPosition;
        }

        protected override void OnClose() { }
        protected override void Localize() { }

        // private void OnApplicationFocus(bool hasFocus)
        // {
        //     if (hasFocus)
        //     {
        //         SlowUpdateSlot(_openedSlot).Forget();
        //         _openedSlot = null;
        //     }
        // }
        //
        // private async UniTaskVoid SlowUpdateSlot(ShopSlotBase slot)
        // {
        //     if (slot == null)
        //         return;
        //     
        //     slot.ShowWaitAnimation();
        //     await AllServices.Container.Single<IUserService>().UpdateOrder(slot.Data);
        //     slot.SetupYourSelf();
        //     slot.HideWaitAnimation();
        //     Refresh();
        // }
    }
}