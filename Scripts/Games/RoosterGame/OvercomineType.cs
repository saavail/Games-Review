using System;

namespace Games.RoosterGame
{
    [Flags]
    public enum OvercomingType
    {
        Nothing = 1,
        Any = 2,
        Roll = 4,
        Jump = 8,
    }
}