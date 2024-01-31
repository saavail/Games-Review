using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Games.View
{
    public class SummerCell : Cell
    {
        [Serializable]
        private class CellSprite
        {
            public int Value;
            public Sprite Skin;
        }

        [Header("Summer")]
        [SerializeField]
        private CellSprite[] _skins;
        [SerializeField]
        private Image _skinImage;
        
        protected override void Setup(int value, Color color)
        {
            base.Setup(value, color);

            var skin = _skins.FirstOrDefault(i => i.Value == value);

            _skinImage.enabled = skin != default;

            if (skin != default)
            {
                _skinImage.sprite = skin.Skin;
                _skinImage.SetNativeSize();
            }
        }
    }
}