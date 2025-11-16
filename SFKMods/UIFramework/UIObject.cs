using UnityEngine;
using UnityEngine.UI;

namespace UIFramework
{
    public class UIObject
    {
        public GameObject GameObject;
        public RectTransform Rect;

        public void SetParent(Transform parent)
        {
            if (parent != null)
                Rect.SetParent(parent, false);
        }

        public void SetSize(Vector2 size)
        {
            Rect.sizeDelta = size;
        }

        public void SetPosition(Vector2 anchoredPos)
        {
            Rect.anchoredPosition = anchoredPos;
        }
    }
}
