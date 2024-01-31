using System;

namespace Core.UserStuff
{
    [Serializable]
    public class RemoteBalance
    {
        public UserLevel[] levels;
        public GameBalance[] gamesData;
        public SlotData[] slotsData;
    }
}