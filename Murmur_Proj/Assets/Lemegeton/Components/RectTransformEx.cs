using UnityEngine;

namespace Lemegeton
{
    public static class RectTransformEx
    {
        // Alignment
        private static void SetAlignment(this RectTransform rect, Vector2 value, bool adjustPivot)
        {
            rect.anchorMax = value;
            rect.anchorMin = value;
            if(adjustPivot) rect.pivot = value;
        }
        public static void AlignTopLeft(this RectTransform rect, bool adjustPivot = true)
        {
            rect.SetAlignment(Vector2.up, adjustPivot);
        }
        public static void AlignTopLeft(this RectTransform rect, Vector2 offset, Vector2 size, bool adjustPivot = true)
        {
            rect.SetAlignment(Vector2.up, adjustPivot);
            rect.anchoredPosition = offset;
            rect.sizeDelta = size;
        }
        public static void AlignTop(this RectTransform rect, bool adjustPivot = true)
        {
            rect.SetAlignment(new Vector2(0.5f, 1f), adjustPivot);
        }
        public static void AlignTop(this RectTransform rect, Vector2 offset, Vector2 size, bool adjustPivot = true)
        {
            rect.SetAlignment(new Vector2(0.5f, 1f), adjustPivot);
            rect.anchoredPosition = offset;
            rect.sizeDelta = size;
        }
        public static void AlignTopRight(this RectTransform rect, bool adjustPivot = true)
        {
            rect.SetAlignment(Vector2.one, adjustPivot);
        }
        public static void AlignTopRight(this RectTransform rect, Vector2 offset, Vector2 size, bool adjustPivot = true)
        {
            rect.SetAlignment(Vector2.one, adjustPivot);
            rect.anchoredPosition = offset;
            rect.sizeDelta = size;
        }
        public static void AlignLeft(this RectTransform rect, bool adjustPivot = true)
        {
            rect.SetAlignment(new Vector2(0, 0.5f), adjustPivot);
        }
        public static void AlignLeft(this RectTransform rect, Vector2 offset, Vector2 size, bool adjustPivot = true)
        {
            rect.SetAlignment(new Vector2(0, 0.5f), adjustPivot);
            rect.anchoredPosition = offset;
            rect.sizeDelta = size;
        }
        public static void AlignCenter(this RectTransform rect, bool adjustPivot = true)
        {
            rect.SetAlignment(new Vector2(0.5f, 0.5f), adjustPivot);
        }
        public static void AlignCenter(this RectTransform rect, Vector2 offset, Vector2 size, bool adjustPivot = true)
        {
            rect.SetAlignment(new Vector2(0.5f, 0.5f), adjustPivot);
            rect.anchoredPosition = offset;
            rect.sizeDelta = size;
        }
        public static void AlignRight(this RectTransform rect, bool adjustPivot = true)
        {
            rect.SetAlignment(new Vector2(1f, 0.5f), adjustPivot);
        }
        public static void AlignRight(this RectTransform rect, Vector2 offset, Vector2 size, bool adjustPivot = true)
        {
            rect.SetAlignment(new Vector2(1f, 0.5f), adjustPivot);
            rect.anchoredPosition = offset;
            rect.sizeDelta = size;
        }
        public static void AlignBottomLeft(this RectTransform rect, bool adjustPivot = true)
        {
            rect.SetAlignment(Vector2.zero, adjustPivot);
        }
        public static void AlignBottomLeft(this RectTransform rect, Vector2 offset, Vector2 size, bool adjustPivot = true)
        {
            rect.SetAlignment(Vector2.zero, adjustPivot);
            rect.anchoredPosition = offset;
            rect.sizeDelta = size;
        }
        public static void AlignBottom(this RectTransform rect, bool adjustPivot = true)
        {
            rect.SetAlignment(new Vector2(0.5f, 0f), adjustPivot);
        }
        public static void AlignBottom(this RectTransform rect, Vector2 offset, Vector2 size, bool adjustPivot = true)
        {
            rect.SetAlignment(new Vector2(0.5f, 0f), adjustPivot);
            rect.anchoredPosition = offset;
            rect.sizeDelta = size;
        }
        public static void AlignBottomRight(this RectTransform rect, bool adjustPivot = true)
        {
            rect.SetAlignment(Vector2.right, adjustPivot);
        }
        public static void AlignBottomRight(this RectTransform rect, Vector2 offset, Vector2 size, bool adjustPivot = true)
        {
            rect.SetAlignment(Vector2.right, adjustPivot);
            rect.anchoredPosition = offset;
            rect.sizeDelta = size;
        }
        // Stretch Horizontal
        private static void SetStretchHorizontalRect(this RectTransform rect, float paddingLeft, float offsetY, float paddingRight, float height)
        {
            rect.offsetMin = new Vector2(paddingLeft, rect.offsetMin.y);
            rect.offsetMax = new Vector2(-paddingRight, rect.offsetMax.y);
            rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, offsetY);
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, height);
        }

        public static void StretchHorizontalTop(this RectTransform rect, float paddingLeft, float offsetY, float paddingRight, float height, bool adjustPivot)
        {
            rect.anchorMin = Vector2.up;
            rect.anchorMax = Vector2.one;
            if(adjustPivot) {
                rect.pivot = new Vector2(rect.pivot.x, 1f);
            }
            rect.SetStretchHorizontalRect(paddingLeft, offsetY, paddingRight, height);
        }
        public static void StretchHorizontalBottom(this RectTransform rect, float paddingLeft, float offsetY, float paddingRight, float height, bool adjustPivot)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.right;
            if(adjustPivot) {
                rect.pivot = new Vector2(rect.pivot.x, 0f);
            }
            rect.SetStretchHorizontalRect(paddingLeft, offsetY, paddingRight, height);
        }
        public static void StretchHorizontalCenter(this RectTransform rect, float paddingLeft, float offsetY, float paddingRight, float height, bool adjustPivot)
        {
            rect.anchorMin = new Vector2(0f, 0.5f);
            rect.anchorMax = new Vector2(1f, 0.5f);
            if(adjustPivot) {
                rect.pivot = new Vector2(rect.pivot.x, 0.5f);
            }
            rect.SetStretchHorizontalRect(paddingLeft, offsetY, paddingRight, height);
        }
        // Stretch Vertical
        private static void SetStretchVerticalRect(this RectTransform rect, float offsetX, float paddingTop, float width, float paddingBottom)
        {
            rect.offsetMin = new Vector2(rect.offsetMin.x, paddingBottom);
            rect.offsetMax = new Vector2(rect.offsetMax.y, -paddingTop);
            rect.anchoredPosition = new Vector2(offsetX, rect.anchoredPosition.y);
            rect.sizeDelta = new Vector2(width, rect.sizeDelta.y);
        }
        public static void StretchVerticalLeft(this RectTransform rect, float offsetX, float paddingTop, float width, float paddingBottom, bool adjustPivot)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.up;
            if(adjustPivot) {
                rect.pivot = new Vector2(0f, rect.pivot.y);
            }
            rect.SetStretchVerticalRect(offsetX, paddingTop, width, paddingBottom);
        }
        public static void StretchVerticalCenter(this RectTransform rect, float offsetX, float paddingTop, float width, float paddingBottom, bool adjustPivot)
        {
            rect.anchorMin = new Vector2(0.5f, 0f);
            rect.anchorMax = new Vector2(0.5f, 1f);
            if(adjustPivot) {
                rect.pivot = new Vector2(0.5f, rect.pivot.y);
            }
            rect.SetStretchVerticalRect(offsetX, paddingTop, width, paddingBottom);
        }
        public static void StretchVerticalRight(this RectTransform rect, float offsetX, float paddingTop, float width, float paddingBottom, bool adjustPivot)
        {
            rect.anchorMin = Vector2.right;
            rect.anchorMax = Vector2.one;
            if(adjustPivot) {
                rect.pivot = new Vector2(1f, rect.pivot.y);
            }
            rect.SetStretchVerticalRect(offsetX, paddingTop, width, paddingBottom);
        }
        // Stretch Full
        public static void StretchFull(this RectTransform rect, float paddingLeft, float paddingTop, float paddingRight, float paddingBottom)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = new Vector2(paddingLeft, paddingBottom);
            rect.offsetMax = new Vector2(-paddingRight, -paddingTop);
        }
        private static int CountCornersVisibleFrom(this RectTransform rectTransform, Camera camera)
        {
            Rect screenBounds = new Rect(0f, 0f, Screen.width, Screen.height);
            Vector3[] objectCorners = new Vector3[4];
            rectTransform.GetWorldCorners(objectCorners);
    
            int visibleCorners = 0;
            Vector3 tempScreenSpaceCorner;
            for (var i = 0; i < objectCorners.Length; i++) {
                tempScreenSpaceCorner = camera.WorldToScreenPoint(objectCorners[i]);
                if(screenBounds.Contains(tempScreenSpaceCorner)) visibleCorners++;
            }
            return visibleCorners;
        }
        public static bool IsFullyVisibleFrom(this RectTransform rectTransform, Camera camera)
        {
            return CountCornersVisibleFrom(rectTransform, camera) == 4; // True if all 4 corners are visible
        }
        public static bool IsVisibleFrom(this RectTransform rectTransform, Camera camera)
        {
            return CountCornersVisibleFrom(rectTransform, camera) > 0; // True if any corners are visible
        }
    }
}

