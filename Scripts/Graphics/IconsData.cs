using UnityEngine;

namespace Graphics
{
    [CreateAssetMenu(menuName = "Scriptables/" + nameof(IconsData), fileName = nameof(IconsData))]
    public class IconsData : ScriptableObject
    {
        [SerializeField]
        private Sprite[] _allIcons = null;

        public Sprite[] AllIcons => _allIcons;
        
#if UNITY_EDITOR
        public void ReplaceIcons(Sprite[] newIcons)
        {
            _allIcons = newIcons;
        }
#endif
    }
}