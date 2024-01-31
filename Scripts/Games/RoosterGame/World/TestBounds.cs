using Sirenix.OdinInspector;
using UnityEngine;

namespace Games.RoosterGame.World
{
    public class TestBounds : MonoBehaviour
    {
        [Button]
        public void Foo()
        {
            Debug.Log(GetComponent<SpriteRenderer>().bounds.size);
            Debug.Log(GetComponent<SpriteRenderer>().bounds.center);
            Debug.Log(GetComponent<SpriteRenderer>().bounds.min);
            Debug.Log(GetComponent<SpriteRenderer>().bounds.max);
            
            Debug.Log(GetComponent<SpriteRenderer>().localBounds.size);
            Debug.Log(GetComponent<SpriteRenderer>().localBounds.center);
            Debug.Log(GetComponent<SpriteRenderer>().localBounds.min);
            Debug.Log(GetComponent<SpriteRenderer>().localBounds.max);
        }
    }
}