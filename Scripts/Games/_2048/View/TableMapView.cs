using System.Collections.Generic;
using System.Linq;
using Core;
using Core.UserStuff;
using DependencyInjector;
using UnityEngine;
using Utilities;

namespace Games.View
{
    public class TableMapView : MonoBehaviour, ITableMapEvents, IGameRestartable
    {
        [SerializeField]
        private Transform _cellRoot;
        [SerializeField]
        private Cell _cellPrefab;
        
        [Header("Settings")]
        [SerializeField]
        private float _spacing;
        [SerializeField]
        private float _cellHeight;
        [SerializeField]
        private float _cellWidth;

        private readonly List<Cell> _cells = new();
        private CellColorsData _cellColors;

        public void SetPrefab(Cell cell)
        {
            _cellPrefab = cell;
        }

        public void SetColors(CellColorsData cellColors)
        {
            _cellColors = cellColors;
        }

        private Vector2 GetPosition(int x, int y)
        {
            return new Vector2(
                x * _spacing + (x + 0.5f) * _cellWidth,
                y * _spacing + (y + 0.5f) * _cellHeight);
        }
        
        private Cell CreateCell()
            => Instantiate(_cellPrefab, _cellRoot);

        private Cell GetCell(int x, int y)
            => _cells.FirstOrDefault(i => i.X == x && i.Y == y);

        void ITableMapEvents.OnPush(int x, int y, int value)
        {
            Cell cell = CreateCell();
            
            var rect = (RectTransform) cell.transform;
            rect.SetAnchor(AnchorPresets.BottomLeft);
            rect.sizeDelta = new Vector2(_cellWidth, _cellHeight);
            rect.anchoredPosition = GetPosition(x, y);
            
            cell.Push(x, y, value, _cellColors.ColorByValue(value));
            _cells.Add(cell);
        }

        void ITableMapEvents.OnMove(int x1, int y1, int x2, int y2)
        {
            GetCell(x1, y1).Move(x2, y2, GetPosition(x2, y2));
        }

        void ITableMapEvents.OnMerge(int x1, int y1, int x2, int y2, int value)
        {
            Cell cell = GetCell(x1, y1);
            cell.Remove(GetPosition(x2, y2), _ => Destroy(cell.gameObject));
            _cells.Remove(cell);
            GetCell(x2, y2).Merge(value, _cellColors.ColorByValue(value));
        }

        void ITableMapEvents.OnDelete(int x, int y)
        {
            var cell = GetCell(x, y);
            _cells.Remove(cell);
            cell.Delete(() => Destroy(cell.gameObject));
        }

        void IGameRestartable.OnRestart(bool isRevive)
        {
            if (isRevive)
                return;

            foreach (var cell in _cells) 
                Destroy(cell.gameObject);

            _cells.Clear();
        }
    }
}