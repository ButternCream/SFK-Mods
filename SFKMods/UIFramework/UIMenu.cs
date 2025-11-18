using UnityEngine;
using UnityEngine.UI;

namespace UIFramework
{
    public class UIMenu : UIObject
    {
        public Image Background;

        public static UIMenu Create(Vector2 size, Vector2 pos, Transform parent, Color? bgColor = null)
        {
            var menu = new UIMenu();

            menu.GameObject = new GameObject("UIMenu");
            menu.Rect = menu.GameObject.AddComponent<RectTransform>();
            menu.SetParent(parent);

            menu.SetSize(size);
            menu.SetPosition(pos);

            // Panel background
            menu.Background = menu.GameObject.AddComponent<Image>();
            menu.Background.color = bgColor ?? new Color(0, 0, 0, 0.0f);

            // Render behind children
            menu.Background.transform.SetAsFirstSibling();

            return menu;
        }

        // Shortcut for enabling vertical layout
        public UIVerticalLayout AddVerticalLayout(float spacing = 8f, float padding = 10f)
        {
            return UIVerticalLayout.AttachTo(this, spacing, padding);
        }

        public UIHorizontalLayout AddHorizontalLayout(float spacing = 8f, float padding = 10f)
        {
            return UIHorizontalLayout.AttachTo(this, spacing, padding);
        }

    }
}
