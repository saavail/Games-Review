using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace UISystem
{
    [CreateAssetMenu(menuName = "Scriptables/" + nameof(WindowData), fileName = nameof(WindowData))]
    public class WindowData : ScriptableObject
    {
        [SerializeField]
        private Window[] _popups;

        [SerializeField]
        private UIHolder _uiHolderPrefab;

        [SerializeField]
        private UserProfile _userProfilePrefab;

        public Window[] Popups => _popups;
        public UIHolder UIHolderPrefab => _uiHolderPrefab;
        public UserProfile UserProfilePrefab => _userProfilePrefab;

#if UNITY_EDITOR
        [Button(ButtonSizes.Medium)]
        public void CollectPopups()
        {
            _popups = AssetDatabase.GetAllAssetPaths()
                .Where(i => i.Contains(".prefab"))
                .Select(path => AssetDatabase.LoadMainAssetAtPath(path) as GameObject)
                .Where(i => i != null && i.GetComponent<Window>() != null)
                .Select(i => i.GetComponent<Window>())
                .ToArray();
            
            AssetDatabase.SaveAssets();
        }
#endif
    }
}