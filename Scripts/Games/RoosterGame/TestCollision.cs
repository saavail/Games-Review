using System;
using UnityEngine;

namespace Games.RoosterGame
{
    public class TestCollision : MonoBehaviour
    {
        private void OnTriggerStay2D(Collider2D other)
        {
            UnityEngine.Debug.Log("trigger stay");
        }

        private void OnCollisionStay2D(Collision2D other)
        {
            Debug.Log("collision stay");
        }
    }
}