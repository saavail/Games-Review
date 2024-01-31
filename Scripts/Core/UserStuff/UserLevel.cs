using System;

namespace Core.UserStuff
{
    [Serializable]
    public struct UserLevel
    {
        public int requiredExperience;
        public Currency reward;
    }
}