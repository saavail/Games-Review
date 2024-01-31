using System;
using Enums;
using UnityEngine.Serialization;

namespace Core.UserStuff
{
    [Serializable]
    public class Skin
    {
        public int skinId;
        public GameType gameType;
        public SkinType skinCollectionType;
        public SkinPart skinType;
        public string skinName;
        public bool isSelected;
    }
}