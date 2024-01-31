using System;
using Games.RoosterGame.Core;
using UnityEngine;

namespace Games.RoosterGame
{
    public class CameraFollow : MonoBehaviour, IRunnerCreated
    {
        [SerializeField]
        private Camera _camera;

        [Header("Settings")]
        [SerializeField]
        private float _offsetY;

        private Transform _target;
        
        private void LateUpdate()
        {
            if (_target == null)
                return;

            Vector3 startPosition = transform.position;
            startPosition.y = _target.position.y + _offsetY;
            transform.position = startPosition;
        }

        public void OnCreate(Rooster rooster)
        {
            _target = rooster.transform;
        }
    }
}