using System.Collections.Generic;
using UnityEngine;

namespace Utilities
{
    public static class RectExtensions
    {
        /// <summary>
        /// Проверяет, что текущий <see cref="RectInt"/> содержит другой <see cref="RectInt"/>.
        /// </summary>
        public static bool ContainsInclusive(this RectInt current, RectInt other)
        {
            // если нижний левый и правый верхний углы внутри, то весь Rect внутри
            return ContainsInclusive(current, other.min) && ContainsInclusive(current, other.max);
        }

        /// <summary>
        /// В отличие от оригинального <see cref="RectInt.Contains(Vector2Int)"/>, точка считается входящей, в том числе, если находится на границе <see cref="RectInt.max"/>.
        /// </summary>
        public static bool ContainsInclusive(this RectInt current, Vector2Int position)
        {
            return position.x >= current.xMin && position.y >= current.yMin && position.x <= current.xMax && position.y <= current.yMax;
        }

        /// <summary>
        /// Возвращает все точки внутри <see cref="RectInt"/>, включая точки, находящиеся на его границе.
        /// 
        /// В отличие от <see cref="RectInt.allPositionsWithin"/>, точка считается входящей, в том числе, если находится на границе <see cref="RectInt.max"/>.
        /// 
        /// Таким образом, RectInt размером 1х1 будет содержать 4 точки (0, 0), (1, 0), (0, 1), (1, 1), а RectInt 2x2 - 9 точек и т.д.
        /// </summary>
        public static IEnumerable<Vector2Int> GetAllPositionsInclusive(this RectInt current)
        {
            for (int y = current.min.y; y <= current.max.y; y++)
            {
                for (int x = current.min.x; x <= current.max.x; x++)
                {
                    yield return new Vector2Int(x, y);
                }
            }
        }

        public static void SetLeft(this RectTransform rt, float left) => 
            rt.offsetMin = new Vector2(left, rt.offsetMin.y);

        public static void SetRight(this RectTransform rt, float right) => 
            rt.offsetMax = new Vector2(-right, rt.offsetMax.y);

        public static void SetTop(this RectTransform rt, float top) => 
            rt.offsetMax = new Vector2(rt.offsetMax.x, -top);

        public static void SetBottom(this RectTransform rt, float bottom) => 
            rt.offsetMin = new Vector2(rt.offsetMin.x, bottom);
    }
}