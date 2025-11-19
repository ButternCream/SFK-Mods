using UnityEngine;
using System.Collections.Generic;

namespace SFKUILib
{
    public class UIVerticalLayout
    {
        private readonly UIMenu menu;
        private readonly float spacing;
        private readonly float padding;

        private readonly List<UIObject> children = new();

        private float currentY;

        private UIVerticalLayout(UIMenu menu, float spacing, float padding)
        {
            this.menu = menu;
            this.spacing = spacing;
            this.padding = padding;

            currentY = -padding;
        }

        public static UIVerticalLayout AttachTo(UIMenu menu, float spacing = 8f, float padding = 10f)
        {
            return new UIVerticalLayout(menu, spacing, padding);
        }

        // Adds a generic UIObject to the layout
        public void Add(UIObject obj)
        {
            obj.SetParent(menu.Rect);

            // Position child
            obj.SetPosition(new Vector2(0, currentY));

            // Move Y cursor (downwards)
            currentY -= obj.Rect.sizeDelta.y + spacing;

            children.Add(obj);
        }

        // Convenience: Add a button directly
        public UIButton AddButton(
            string text,
            Vector2 size,
            Color? bgColor = null,
            bool rounded = false,
            bool enableShake = false)
        {
            var btn = UIButton.Create(
                text: text,
                parent: menu.Rect,
                size: size,
                pos: Vector2.zero,
                bgColor: bgColor,
                rounded: rounded,
                enableShake: enableShake
            );

            Add(btn);
            return btn;
        }

        // Optional: auto-size menu to fit children
        public void FitMenuHeight()
        {
            float totalHeight = padding + Mathf.Abs(currentY);
            menu.SetSize(new Vector2(menu.Rect.sizeDelta.x, totalHeight));
        }
    }
}
