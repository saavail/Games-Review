using System;
using System.Linq;
using Core;
using Enums;
using UnityEngine;
using UnityEngine.UI;

namespace Games.View
{
    public class Game2048SkinChanger : SkinChanger
    {
        [Serializable]
        private struct CellSkin
        {
            public SkinType SkinType;
            public Cell Cell;
        }
        
        [Serializable]
        private class CellColor
        {
            public SkinType SkinType;
            public Color Color;
        }
        
        [SerializeField]
        private Image _background;
        [SerializeField]
        private Image[] _cellsBackgrounds;
        
        [SerializeField]
        private TableMapView _tableMapView;
        [SerializeField]
        private CellSkin[] _cellSkins;

        [SerializeField]
        private CellColor[] _cellColors;
        
        protected override GameType GameType => GameType.Game2048;
        
        protected override void OnStart(GameData gameData)
        {
            _background.sprite = GetSkin(SkinPart.Background);

            Sprite cellBackgroundSkin = GetSkin(SkinPart.Character);
            SkinType characterSkinType = GetSkinType(SkinPart.Character);

            Color color = _cellColors.FirstOrDefault(i => i.SkinType == characterSkinType)?.Color ?? Color.white;

            foreach (Image cellBackground in _cellsBackgrounds)
            {
                cellBackground.sprite = cellBackgroundSkin;
                cellBackground.color = color;
            }

            var skinCell = _cellSkins.First(i => i.SkinType == characterSkinType).Cell;
            _tableMapView.SetPrefab(skinCell);

            var colors = ((Game2048Data) gameData).ColorsBySkin(_user.GetChooseSkin(GameType, SkinPart.Character)) 
                         ?? ((Game2048Data) gameData).ColorsBySkin(SkinType.Default);

            _tableMapView.SetColors(colors);
        }
    }
}