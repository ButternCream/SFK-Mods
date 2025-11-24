using UnityEngine;

namespace SFKUILib
{
    public class UIObject : IUIContainer
    {
        public GameObject GameObject;
        public RectTransform Rect { get; set; }

        public bool Active
        {
            set => GameObject.SetActive(value);
            get => GameObject.activeSelf;
        }

        public void SetParent(Transform parent)
        {
            if (parent != null)
            {
                Rect.SetParent(parent);
            }
        }

        public void SetSize(Vector2 size)
        {
            Rect.sizeDelta = size;
        }

        public void SetPosition(Vector2 anchoredPos)
        {
            Rect.anchoredPosition = anchoredPos;
        }

        public void Hide()
        {
            GameObject.SetActive(false);
        }

        public void Show()
        {
            GameObject.SetActive(true);
        }
    }
}
