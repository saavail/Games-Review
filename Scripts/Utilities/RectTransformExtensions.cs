using UnityEngine;

namespace Utilities
{
    public enum AnchorPresets
    {
        TopLeft,
        TopCenter,
        TopRight,

        MiddleLeft,
        MiddleCenter,
        MiddleRight,

        BottomLeft,
        BottonCenter,
        BottomRight,
        BottomStretch,

        VertStretchLeft,
        VertStretchRight,
        VertStretchCenter,

        HorStretchTop,
        HorStretchMiddle,
        HorStretchBottom,

        StretchAll
    }

    public enum PivotPresets
    {
        TopLeft,
        TopCenter,
        TopRight,

        MiddleLeft,
        MiddleCenter,
        MiddleRight,

        BottomLeft,
        BottomCenter,
        BottomRight,
    }

    public static class RectTransformExtensions
    {
        public static void SetAnchor(this RectTransform source, AnchorPresets allign, Vector2? anchoredPosition = null, Vector2? sizeDelta = null)
        {
            source.anchoredPosition = anchoredPosition.HasValue ? anchoredPosition.Value : Vector2.zero;
            source.sizeDelta = sizeDelta.HasValue ? sizeDelta.Value : Vector2.zero;

            switch (allign)
            {
                case (AnchorPresets.TopLeft):
                {
                    source.anchorMin = new Vector2(0, 1);
                    source.anchorMax = new Vector2(0, 1);
                    break;
                }
                case (AnchorPresets.TopCenter):
                {
                    source.anchorMin = new Vector2(0.5f, 1);
                    source.anchorMax = new Vector2(0.5f, 1);
                    break;
                }
                case (AnchorPresets.TopRight):
                {
                    source.anchorMin = new Vector2(1, 1);
                    source.anchorMax = new Vector2(1, 1);
                    break;
                }

                case (AnchorPresets.MiddleLeft):
                {
                    source.anchorMin = new Vector2(0, 0.5f);
                    source.anchorMax = new Vector2(0, 0.5f);
                    break;
                }
                case (AnchorPresets.MiddleCenter):
                {
                    source.anchorMin = new Vector2(0.5f, 0.5f);
                    source.anchorMax = new Vector2(0.5f, 0.5f);
                    break;
                }
                case (AnchorPresets.MiddleRight):
                {
                    source.anchorMin = new Vector2(1, 0.5f);
                    source.anchorMax = new Vector2(1, 0.5f);
                    break;
                }

                case (AnchorPresets.BottomLeft):
                {
                    source.anchorMin = new Vector2(0, 0);
                    source.anchorMax = new Vector2(0, 0);
                    break;
                }
                case (AnchorPresets.BottonCenter):
                {
                    source.anchorMin = new Vector2(0.5f, 0);
                    source.anchorMax = new Vector2(0.5f, 0);
                    break;
                }
                case (AnchorPresets.BottomRight):
                {
                    source.anchorMin = new Vector2(1, 0);
                    source.anchorMax = new Vector2(1, 0);
                    break;
                }

                case (AnchorPresets.HorStretchTop):
                {
                    source.anchorMin = new Vector2(0, 1);
                    source.anchorMax = new Vector2(1, 1);
                    break;
                }
                case (AnchorPresets.HorStretchMiddle):
                {
                    source.anchorMin = new Vector2(0, 0.5f);
                    source.anchorMax = new Vector2(1, 0.5f);
                    break;
                }
                case (AnchorPresets.HorStretchBottom):
                {
                    source.anchorMin = new Vector2(0, 0);
                    source.anchorMax = new Vector2(1, 0);
                    break;
                }

                case (AnchorPresets.VertStretchLeft):
                {
                    source.anchorMin = new Vector2(0, 0);
                    source.anchorMax = new Vector2(0, 1);
                    break;
                }
                case (AnchorPresets.VertStretchCenter):
                {
                    source.anchorMin = new Vector2(0.5f, 0);
                    source.anchorMax = new Vector2(0.5f, 1);
                    break;
                }
                case (AnchorPresets.VertStretchRight):
                {
                    source.anchorMin = new Vector2(1, 0);
                    source.anchorMax = new Vector2(1, 1);
                    break;
                }

                case (AnchorPresets.StretchAll):
                {
                    source.anchorMin = new Vector2(0, 0);
                    source.anchorMax = new Vector2(1, 1);
                    break;
                }
            }
        }

        public static void SetPivot(this RectTransform source, PivotPresets preset)
        {

            switch (preset)
            {
                case (PivotPresets.TopLeft):
                {
                    source.pivot = new Vector2(0, 1);
                    break;
                }
                case (PivotPresets.TopCenter):
                {
                    source.pivot = new Vector2(0.5f, 1);
                    break;
                }
                case (PivotPresets.TopRight):
                {
                    source.pivot = new Vector2(1, 1);
                    break;
                }

                case (PivotPresets.MiddleLeft):
                {
                    source.pivot = new Vector2(0, 0.5f);
                    break;
                }
                case (PivotPresets.MiddleCenter):
                {
                    source.pivot = new Vector2(0.5f, 0.5f);
                    break;
                }
                case (PivotPresets.MiddleRight):
                {
                    source.pivot = new Vector2(1, 0.5f);
                    break;
                }

                case (PivotPresets.BottomLeft):
                {
                    source.pivot = new Vector2(0, 0);
                    break;
                }
                case (PivotPresets.BottomCenter):
                {
                    source.pivot = new Vector2(0.5f, 0);
                    break;
                }
                case (PivotPresets.BottomRight):
                {
                    source.pivot = new Vector2(1, 0);
                    break;
                }
            }
        }

        /// <summary>
        /// Determines if this RectTransform is fully visible from the specified camera.
        /// Works by checking if each bounding box corner of this RectTransform is inside the cameras screen space view frustrum.
        /// </summary>
        /// <returns><c>true</c> if is fully visible from the specified camera; otherwise, <c>false</c>.</returns>
        /// <param name="rectTransform">Rect transform.</param>
        /// <param name="camera">Camera.</param>
        public static bool IsFullyVisibleFrom(this RectTransform rectTransform, Camera camera, RectTransform viewport = null)
        {
            return CountCornersVisibleFrom(rectTransform, camera, viewport) == 4; // True if all 4 corners are visible
        }
    
        /// <summary>
        /// Determines if this RectTransform is at least partially visible from the specified camera.
        /// Works by checking if any bounding box corner of this RectTransform is inside the cameras screen space view frustrum.
        /// </summary>
        /// <returns><c>true</c> if is at least partially visible from the specified camera; otherwise, <c>false</c>.</returns>
        /// <param name="rectTransform">Rect transform.</param>
        /// <param name="camera">Camera.</param>
        public static bool IsVisibleFrom(this RectTransform rectTransform, Camera camera, RectTransform viewport = null)
        {
            return CountCornersVisibleFrom(rectTransform, camera, viewport) > 0; // True if any corners are visible
        }

        /// <summary>
        /// Counts the bounding box corners of the given RectTransform that are visible from the given Camera in screen space.
        /// </summary>
        /// <returns>The amount of bounding box corners that are visible from the Camera.</returns>
        /// <param name="rectTransform">Rect transform.</param>
        /// <param name="camera">Camera.</param>
        private static int CountCornersVisibleFrom(this RectTransform rectTransform, Camera camera, RectTransform viewport = null)
        {
            if (!rectTransform.gameObject.activeInHierarchy)
                return 0;

            Rect rectBounds;
            if (viewport != null)
            {
                throw new System.Exception("not support yet!");
            }
            else
            {
                rectBounds = new Rect(0f, 0f, Screen.width, Screen.height);
            }
        
            Vector3[] objectCorners = new Vector3[4];
            rectTransform.GetWorldCorners(objectCorners);
        
            int visibleCorners = 0;
            Vector3 corner;
            for (var i = 0; i < objectCorners.Length; i++)
            {
                corner = camera.WorldToScreenPoint(objectCorners[i]);
                if (rectBounds.Contains(corner))
                {
                    visibleCorners++;
                }
            }

            return visibleCorners;
        }

        public static Rect RectTransformToScreenSpace(this RectTransform transform)
        {
            Vector2 size = Vector2.Scale(transform.rect.size, transform.lossyScale);
            float x = transform.position.x + transform.anchoredPosition.x;
            float y = Screen.height - transform.position.y - transform.anchoredPosition.y;

            return new Rect(x, y, size.x, size.y);
        }

        public static void SetViewportPosition(this RectTransform rect, Vector2 viewportPosition)
        {
            rect.anchorMax = viewportPosition;
            rect.anchorMin = viewportPosition;

            rect.anchoredPosition = viewportPosition;
        }
    }
}