using UnityEngine;
using UnityEngine.EventSystems;

namespace UISystem
{
    public class UIHolder : MonoBehaviour
    {
        [SerializeField]
        private Transform _windowsRoot;
        [SerializeField]
        private Camera _camera;
        [SerializeField]
        private EventSystem _eventSystem;

        public Transform WindowsRoot => _windowsRoot;
        public Camera Camera => _camera;
        public EventSystem EventSystem => _eventSystem;
    }
}