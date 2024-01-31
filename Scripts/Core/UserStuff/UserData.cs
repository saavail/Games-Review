using System.Collections.Generic;
using UnityEngine;

namespace Core.UserStuff
{
    [CreateAssetMenu(menuName = "Scriptables/" + nameof(UserData), fileName = nameof(UserData))]
    public class UserData : ScriptableObject
    {
        [SerializeField]
        private List<Currency> _money;

        public List<Currency> Money => _money;
    }
}