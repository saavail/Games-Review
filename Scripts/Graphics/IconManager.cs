using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using EntryPoint;
using Enums;
using UnityEngine;

namespace Graphics
{
    public class IconManager : AsyncInitializableAndLoad<IconsData>, IIconsService
    {
        public const string EmptyKey = "Empty";

        private Dictionary<string, Sprite> _icons;

        private readonly Dictionary<CurrencyType, string> _currency = new()
        {
            {CurrencyType.Gold, "Gold"},
            {CurrencyType.Gems, "Gem"},
            {CurrencyType.Ticket, "Ticket"},
            {CurrencyType.USD, EmptyKey},
            {CurrencyType.USDTest, EmptyKey},
        };

        private readonly Dictionary<(GameType game, SkinType skinType, SkinPart part), string[]> _skins = new()
        {
            // Default
            {(GameType.FlappyPlane, SkinType.Default, SkinPart.Character), new[] {"FlappyPlaneDefaultCharacter"}},
            {(GameType.FlappyPlane, SkinType.Default, SkinPart.Background), new[]
            {
                "FlappyPlaneDefaultBack1", 
                "FlappyPlaneDefaultBack2", 
                "FlappyPlaneDefaultBack3", 
            }},
            {(GameType.FlappyPlane, SkinType.Default, SkinPart.Environment), new[] { "FlappyPlaneDefaultEnvironment1", }},
            // Summer
            {(GameType.FlappyPlane, SkinType.Summer, SkinPart.Character), new[] {"FlappyPlaneSummerCharacter"}},
            {(GameType.FlappyPlane, SkinType.Summer, SkinPart.Background), new[]
            {
                "SummerBack", 
                "FlappyPlaneSummerBack2", 
                "FlappyPlaneSummerBack3"
            }},
            {(GameType.FlappyPlane, SkinType.Summer, SkinPart.Environment), new[] {"FlappyPlaneSummerEnvironment"}},
            // Winter
            {(GameType.FlappyPlane, SkinType.Winter, SkinPart.Character), new[] {"FlappyPlaneWinterCharacter"}},
            {(GameType.FlappyPlane, SkinType.Winter, SkinPart.Background), new[]
            {
                "FlappyPlaneWinterBack1", 
                "FlappyPlaneWinterBack2", 
                "FlappyPlaneWinterBack3"
            }},
            {(GameType.FlappyPlane, SkinType.Winter, SkinPart.Environment), new[] {"FlappyPlaneWinterEnvironment"}},

            // Default
            {(GameType.Game2048, SkinType.Default, SkinPart.Character), new[] {"CellFlat"}},
            {(GameType.Game2048, SkinType.Default, SkinPart.Background), new[] {"2048DefaultBack"}},
            // Summer
            {(GameType.Game2048, SkinType.Summer, SkinPart.Character), new[] {"CellFlat"}},
            {(GameType.Game2048, SkinType.Summer, SkinPart.Background), new[] {"SummerBack"}},
            // Winter
            {(GameType.Game2048, SkinType.Winter, SkinPart.Character), new[] {"WinterCell"}},
            {(GameType.Game2048, SkinType.Winter, SkinPart.Background), new[] {"2048WinterBack"}},
        };

        private readonly Dictionary<GameType, string> _gameIcons = new()
        {
            {GameType.Game2048, "Game2048Icon"},
            {GameType.FlappyPlane, "а іконкі-03 (1)"},
        };
        
        private readonly Dictionary<CryptoType, string> _cryptoIcons = new()
        {
            {CryptoType.USDT, "curr_Euro"},
            {CryptoType.Ethereum, "curr_Ethereum"},
            {CryptoType.Bitcoin, "curr_Bitcoin"},
            {CryptoType.Crypto1, "curr_CHN"},
            {CryptoType.Crypto2, "curr_Tether"},
            {CryptoType.Crypto3, "curr_Tether"},
            {CryptoType.Crypto4, "curr_Bitcoin"},
            {CryptoType.Crypto5, "curr_Tether"},
            {CryptoType.Crypto6, "curr_CHN"},
        };
        
        // порядок обычно такой Character -> Background -> Environment
        private readonly Dictionary<int, string> _shopPreview = new()
        {
            {1, "GemsShop1"},
            {2, "GemsShop2"},
            {3, "GemsShop3"},
            {4, "GemsShop4"},
            {5, "GemsShop5"},
            {6, "GemsShop6"},
            
            // 2048
            {7, "2048WinterCharacterShop"},
            {8, "WinterBackShop"},
            {9, "2048SummerCharacterShop"},
            {10, "SummerBackShop"},

            // FlappyPlane
            {11, "FlappyPlaneWinterCharacterShop"},
            {12, "FlappyPlaneWinterEnvironmentShop"},
            {13, "WinterBackShop"},
            {14, "FlappyPlaneSummerCharacterShop"},
            {15, "FlappyPlaneSummerEnvironmentShop"},
            {16, "SummerBackShop"},
        };

        public Sprite GetMoneyMessageIcon => GetIcon("MoneyMessage");
        public Sprite GetGameIcon => GetIcon("IconMessage");

        public IconManager(IResourceLoader resourceLoader)
            : base(resourceLoader) { }

        public override async UniTask InitializeAsync()
        {
            await base.InitializeAsync();
            
            _icons = await Data.AllIcons.ToUniTaskAsyncEnumerable().ToDictionaryAsync(i => i.name, i => i);
            _icons.Add(EmptyKey, null);
        }

        public Sprite GetIcon(CurrencyType currency)
            => GetIcon(_currency, currency);

        public Sprite GetIcon(GameType game)
            => GetIcon(_gameIcons, game);

        public Sprite GetIcon(CryptoType crypto)
            => GetIcon(_cryptoIcons, crypto);

        public Sprite GetShopPreview(int slotId)
            => GetIcon(_shopPreview, slotId);

        public Sprite[] GetSkin(GameType gameType, SkinType skinType, SkinPart skinPart)
        {
            if (!_skins.TryGetValue((gameType, skinType, skinPart), out string[] iconsPaths))
            {
                Debug.LogException(new Exception($"Not found {skinType} {skinPart} for {gameType} icon in {nameof(IconManager)}"));
            }

            return iconsPaths?.Select(GetIcon).ToArray();
        }

        public Sprite GetIcon(string name)
        {
            if (!_icons.TryGetValue(name, out var sprite))
            {
                Debug.LogErrorFormat("Icon {0} was not found", name);
            }

            return sprite;
        }

        private Sprite GetIcon<T>(Dictionary<T, string> icons, T type)
        {
            if (!icons.TryGetValue(type, out var itemIconPath))
            {
                Debug.LogException(new Exception($"Not found {type} icon in {nameof(IconManager)}"));
            }

            return GetIcon(itemIconPath);
        }
    }
}