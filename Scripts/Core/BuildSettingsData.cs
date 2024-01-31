using UnityEngine;

namespace Core
{
    using UnityEngine;

    [CreateAssetMenu(menuName = "Scriptables/" + nameof(BuildSettingsData), fileName = nameof(BuildSettingsData))]
    public class BuildSettingsData : ScriptableObject
    {
        public bool IsDevelopment;
    }
}