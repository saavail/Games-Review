using System;
using System.Collections.Generic;
using Enums;
using UnityEngine;
using UnityEngine.UI.Extensions;
using UnityEngine.UI.Extensions.EasingCore;

namespace UISystem.Shop
{
    public class CurrencyScroll : FancyScrollView<CryptoType, Context>
    {
        [SerializeField]
        private Scroller _scroller;
        [SerializeField]
        private GameObject _prefab;

        protected override GameObject CellPrefab => _prefab;
        
        public Action<int> OnSelectionChanged;
        
        protected override void Initialize()
        {
            base.Initialize();

            _scroller.OnValueChanged(UpdatePosition);
            _scroller.OnSelectionChanged(UpdateSelection);
        }

        private void OnEnable()
        {
            Context.OnCellClicked += SelectCell;
        }

        private void OnDisable()
        {
            Context.OnCellClicked -= SelectCell;
        }

        private void UpdateSelection(int index)
        {
            if (Context.SelectedIndex == index)
                return;

            Context.SelectedIndex = index;
            Refresh();
            
            OnSelectionChanged?.Invoke(index);
        }
        
        public void UpdateData(IList<CryptoType> items)
        {
            UpdateContents(items);
            _scroller.SetTotalCount(items.Count);
        }

        public void SelectCell(int index)
        {
            if (index < 0 || index >= ItemsSource.Count || index == Context.SelectedIndex)
                return;
            
            UpdateSelection(index);
            _scroller.ScrollTo(index, 0.35f, Ease.OutCubic);
        }
    }
}