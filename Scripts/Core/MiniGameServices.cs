using UnityEngine;
using UnityEngine.Pool;

namespace Core
{
    public class MiniGameServices
    {
        private readonly GameData _data;

        public MiniGameServices(GameData data)
        {
            _data = data;
        }
    }
}