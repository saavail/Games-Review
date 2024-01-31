using System;
using Enums;

namespace Core.UserStuff
{
    [Serializable]
    public class GameScore
    {
        public GameType gameType;
        public int progress;
    }
}