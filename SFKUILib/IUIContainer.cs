using UnityEngine;

namespace SFKUILib
{
    public interface IUIContainer
    {
        public RectTransform Rect { get; }
        void SetSize(Vector2 size);
        void SetPosition(Vector2 position);
        void SetParent(Transform transform);
        void Hide();
        void Show();
    }

}
