using Sirenix.OdinInspector;
using UnityEngine;

namespace Games.RoosterGame.Core
{
    public class TestAxis : MonoBehaviour
    {
        public TransparencySortMode mode;
        public Vector3 axis;
        
        [Button]
        public void Foo()
        {
            var cam = GetComponent<Camera>();
            cam.transparencySortMode = mode;
            cam.transparencySortAxis = axis;
        }
    }
}