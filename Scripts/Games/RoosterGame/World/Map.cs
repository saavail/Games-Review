using System;
using System.Collections.Generic;
using UnityEngine;

namespace Games.RoosterGame.World
{
    public class Map : MonoBehaviour
    {
        private const float GizmosAlpha = 0.45f;
        
        private static readonly Dictionary<int, Color> ColorByLine = new()
        {
            { 0, new Color(1,1,0,GizmosAlpha) },
            { 1, new Color(0,1,0,GizmosAlpha) },
            { 2, new Color(0,0,1,GizmosAlpha) },
        };
        
        [SerializeField]
        private float _lineWidth;
        [SerializeField]
        private Transform[] _lines;

        public int LinesCount => _lines.Length;

        public float this[int index] => _lines[index].position.x;

        private void OnDrawGizmosSelected()
        {
            for (int i = 0; i < _lines.Length; i++)
            {
                Gizmos.color = ColorByLine[i];
                Gizmos.DrawCube(_lines[i].position + Vector3.up * -999, new Vector3(_lineWidth, 9999f, 0.01f));
            }
        }

        public bool IsAbsolutelyInLine(Vector3 position, int lineIndex) 
            => position.x < (this[lineIndex] + 0.1f) && position.x > (this[lineIndex] - 0.1f);

        public bool IsInLineBounds(Vector3 position, int lineIndex) 
            => position.x > (this[lineIndex] - _lineWidth / 2f) && position.x < (this[lineIndex] + _lineWidth / 2f);

        public int GetLineIndex(Vector3 position)
        {
            for (int i = 0; i < _lines.Length; i++)
            {
                if (IsInLineBounds(position, i))
                {
                    return i;
                }
            }

            return -1;
        }

        private void OnValidate()
        {
            Vector3 position = _lines[1].position;
            position.x = 0;
            _lines[1].position = position;
            
            position = _lines[0].position;
            position.x = _lineWidth * -1;
            _lines[0].position = position;
            
            position = _lines[2].position;
            position.x = _lineWidth;
            _lines[2].position = position;
        }
    }
}