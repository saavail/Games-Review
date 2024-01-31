using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Games
{
    public class TableMap
    {
        private readonly struct Map
        {
            private readonly int[,] _map;

            public int this[int x, int y]
            {
                get => _map[y, x];
                set => _map[y, x] = value;
            }

            public int RowsCount => _map.GetUpperBound(0) + 1;
            public int ColumnsCount => _map.Length / RowsCount;
            
            public Map(int sizeX, int sizeY)
            {
                _map = new int[sizeX, sizeY];
            }
        
            public Map(int size)
            {
                _map = new int[size, size];
            }

            public Map Clone()
            {
                int sizeX = _map.GetUpperBound(0) + 1;
                int sizeY = _map.Length / sizeX;

                var newMap = new Map(sizeX, sizeY);

                for (int x = 0; x < sizeX; x++)
                {
                    for (int y = 0; y < sizeY; y++)
                    {
                        newMap[y, x] = _map[x, y];
                    }
                }

                return newMap;
            }

            public void Clear()
            {
                int sizeX = _map.GetUpperBound(0) + 1;
                int sizeY = _map.Length / sizeX;
                
                for (int x = 0; x < sizeX; x++)
                {
                    for (int y = 0; y < sizeY; y++)
                    {
                        _map[y, x] = EmptyValue;
                    }
                }
            }
        }
        
        private const int EmptyValue = 0;

        private readonly float _cell4Chance;
        private readonly ITableMapEvents[] _eventsReceivers;
        private readonly Map _map;
        private readonly bool _canLog;

        public int RowsCount => _map.RowsCount;
        public int ColumnsCount => _map.ColumnsCount;

        public TableMap(int sizeX, int sizeY, float cell4Chance, ITableMapEvents[] eventsReceivers, bool canLog = false) 
            : this(cell4Chance, eventsReceivers, canLog)
        {
            _map = new Map(sizeX, sizeY);
            InitializeFirstSlots();
        }
        
        public TableMap(int size, float cell4Chance, ITableMapEvents[] eventsReceivers, bool canLog = false) 
            : this(cell4Chance, eventsReceivers, canLog)
        {
            _map = new Map(size);
            InitializeFirstSlots();
        }

        public void Revive()
        {
            for (int x = 0; x < _map.ColumnsCount; x++)
            {
                for (int y = 0; y < _map.RowsCount - 1; y++)
                {
                    if (_map[x, y] > 4) 
                        continue;
                    
                    _map[x, y] = EmptyValue;

                    foreach (var receiver in _eventsReceivers) 
                        receiver.OnDelete(x, y);
                }
            }
        }

        public void RestartGame()
        {
            _map.Clear();
            InitializeFirstSlots();
        }

        private TableMap(float cell4Chance, ITableMapEvents[] eventsReceivers, bool canLog = false)
        {
            _canLog = canLog;
            _cell4Chance = cell4Chance;
            _eventsReceivers = eventsReceivers;
        }

        private void InitializeFirstSlots()
        {
            var slot = GetRandomFreeSlot();
            Push(slot.x, slot.y, 2);
            
            Push();
        }

        private List<Vector2Int> GetFreePositions()
        {
            var points = new List<Vector2Int>(_map.RowsCount * _map.ColumnsCount);

            for (int x = 0; x < _map.ColumnsCount; x++)
            {
                for (int y = 0; y < _map.RowsCount; y++)
                {
                    if (IsFree(x, y))
                    {
                        points.Add(new Vector2Int(x, y));
                    }
                }
            }

            return points;
        }

        private Vector2Int GetRandomFreeSlot()
        {
            var positions = GetFreePositions();
            return positions.Count > 0 ? positions[Random.Range(0, positions.Count)] : -Vector2Int.one;
        }

        public bool IsLose()
        {
            if (GetRandomFreeSlot() != -Vector2Int.one)
                return false;
            
            for (int x = 0; x < _map.ColumnsCount; x++)
            {
                for (int y = 0; y < _map.RowsCount - 1; y++)
                {
                    if (_map[x, y] == _map[x, y + 1])
                    {
                        return false;
                    }
                }
            }

            for (int y = 0; y < _map.RowsCount; y++)
            {
                for (int x = 0; x < _map.ColumnsCount - 1; x++)
                {
                    if (_map[x, y] == _map[x + 1, y])
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private int GetStartValue()
            => Random.value < _cell4Chance ? 4 : 2;

        public bool IsFreePublic(int x, int y)
            => !IsOutOfBorders(x, y) && IsFree(x, y);

        public int GetValue(int x, int y)
            => _map[x, y];

        private bool IsFree(int x, int y)
            => _map[x, y] == EmptyValue;

        public void Push(int x, int y, int value)
        {
            if (!Mathf.IsPowerOfTwo(value) || !IsFreePublic(x, y))
                return;

            _map[x, y] = value;

            foreach (var receiver in _eventsReceivers)
                receiver.OnPush(x, y, value);
        }

        public void Push()
        {
            var slot = GetRandomFreeSlot();
            Push(slot.x, slot.y, GetStartValue());
        }

        public void Swipe(Swipe swipe)
        {
            var lastMap = _map.Clone();
            
            switch (swipe)
            {
                case Games.Swipe.Left:
                {
                    for (int y = 0; y < RowsCount; y++)
                    {
                        var merged = new bool[ColumnsCount];

                        for (int x = 0; x < ColumnsCount; x++)
                        {
                            if (IsFree(x, y))
                                continue;

                            int x1 = x - 1;
                            for (; x1 >= 0; x1--)
                            {
                                if (IsFree(x1, y))
                                    continue;

                                if (_map[x1, y] == _map[x, y] && !merged[x1])
                                {
                                    Merge(x, y, x1, y);
                                    merged[x1] = true;
                                }
                                else
                                {
                                    Move(x, y, x1 + 1, y);
                                }

                                break;
                            }

                            if (IsFree(0, y))
                            {
                                Move(x, y, 0, y);
                            }
                        }
                    }
                }
                    break;

                case Games.Swipe.Right:
                {
                    for (int y = 0; y < RowsCount; y++)
                    {
                        var merged = new bool[ColumnsCount];
                        
                        for (int x = ColumnsCount - 1; x >= 0; x--)
                        {
                            if (IsFree(x, y))
                                continue;

                            int x1 = x + 1;
                            for (; x1 < ColumnsCount; x1++)
                            {
                                if (IsFree(x1, y))
                                    continue;

                                if (_map[x1, y] == _map[x, y])
                                {
                                    Merge(x, y, x1, y);
                                    merged[x1] = true;
                                }
                                else
                                {
                                    Move(x, y, x1 - 1, y);
                                }

                                break;
                            }

                            if (IsFree(ColumnsCount - 1, y))
                            {
                                Move(x, y, ColumnsCount - 1, y);
                            }
                        }
                    }
                }
                    break;

                case Games.Swipe.Up:
                {
                    for (int x = 0; x < ColumnsCount; x++)
                    {
                        var merged = new bool[RowsCount];
                        
                        for (int y = RowsCount - 1; y >= 0; y--)
                        {
                            if (IsFree(x, y))
                                continue;

                            int y1 = y + 1;
                            for (; y1 < RowsCount; y1++)
                            {
                                if (IsFree(x, y1))
                                    continue;

                                if (_map[x, y1] == _map[x, y] && !merged[y1])
                                {
                                    Merge(x, y, x, y1);
                                    merged[y1] = true;
                                }
                                else
                                {
                                    Move(x, y, x, y1 - 1);
                                }

                                break;
                            }

                            if (IsFree(x, RowsCount - 1))
                            {
                                Move(x, y, x, RowsCount - 1);
                            }
                        }
                    }
                }
                    break;

                case Games.Swipe.Down:
                {
                    for (int x = 0; x < ColumnsCount; x++)
                    {
                        var merged = new bool[RowsCount];
                        
                        for (int y = 0; y < RowsCount; y++)
                        {
                            if (IsFree(x, y))
                                continue;

                            int y1 = y - 1;
                            for (; y1 >= 0; y1--)
                            {
                                if (IsFree(x, y1))
                                    continue;

                                if (_map[x, y1] == _map[x, y] && !merged[y1])
                                {
                                    Merge(x, y, x, y1);
                                    merged[y1] = true;
                                }
                                else
                                {
                                    Move(x, y, x, y1 + 1);
                                }

                                break;
                            }

                            if (IsFree(x, 0))
                            {
                                Move(x, y, x, 0);
                            }
                        }
                    }
                }
                    break;
            }
            
            if (!IsEquals(lastMap))
            {
                Push();
                
                if (_canLog)
                {
                    Debug.Log(CreateLog());
                }
            }
        }

        private string CreateLog()
        {
            string log = String.Empty;

            for (int i = RowsCount - 1; i >= 0; i--)
            {
                for (int j = 0; j < ColumnsCount; j++)
                {
                    log += $"{_map[j, i]} ";
                }
                
                if (i != 0)
                    log += "\n";
            }

            return log;
        }

        private bool IsEquals(Map map)
        {
            for (int x = 0; x < ColumnsCount; x++)
            {
                for (int y = 0; y < RowsCount; y++)
                {
                    if (map[x, y] != _map[x, y])
                        return false;
                }
            }

            return true;
        }

        private void Move(int x1, int y1, int x2, int y2)
        {
            if (x1 == x2 && y1 == y2)
                return;

            _map[x2, y2] = _map[x1, y1];
            _map[x1, y1] = EmptyValue;

            foreach (var receiver in _eventsReceivers)
                receiver.OnMove(x1, y1, x2, y2);
        }

        private void Merge(int x1, int y1, int x2, int y2)
        {
            if (x1 == x2 && y1 == y2)
                return;
            
            _map[x1, y1] = EmptyValue;
            _map[x2, y2] *= 2;
            
            foreach (var receiver in _eventsReceivers)
                receiver.OnMerge(x1, y1, x2, y2, _map[x2, y2]);
        }
        
        private bool IsOutOfBorders(int x, int y)
        {
            bool isOutOfBorders = !IsInRange(x, 0, ColumnsCount - 1) || !IsInRange(y, 0, RowsCount - 1);

            if (isOutOfBorders)
            {
                Debug.LogException(new Exception($"1 or 2 values is out of borders map: value 1: {x}, value 2:{y}, Rows: {RowsCount}, Columns: {ColumnsCount}"));
            }
            
            return isOutOfBorders;
        }

        private bool IsInRange(int value, int min, int max)
            => value >= min && value <= max;
    }
}