using System;
using Enums;

namespace Core.UserStuff
{
    [Serializable]
    public struct SlotData
    {
        public int slotId;
        public Currency price;
        public Currency reward;
        public bool reuse;
        public GameType gameType;
        public SkinPart skinType;
        public SkinType skinCollectionType;
        public string skinName;

        public bool IsSkin => skinType != SkinPart.None && skinCollectionType != default && gameType != GameType.None;
        public bool IsDefault => slotId <= 0;
    }
}