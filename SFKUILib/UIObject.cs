using UnityEngine;
using UnityEngine.UI;

namespace SFKUILib
{
    public class UIObject
    {
        public GameObject GameObject;
        public RectTransform Rect;
        public bool Active
        {
            set
            {
                GameObject.SetActive(value);
            }
            get 
            {  
                return GameObject.activeSelf; 
            }
        }

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
