using System;
using Enums;

namespace Core.UserStuff
{
    [Serializable]
    public struct GameBalance
    {
        public GameType gameType;
        public Currency reviveCost;
        public float experienceMultiplier;
    }
}