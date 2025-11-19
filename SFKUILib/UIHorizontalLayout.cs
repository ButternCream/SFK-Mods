using System.Collections.Generic;
using UnityEngine;

namespace SFKUILib
{
    public class UIHorizontalLayout
    {
        private readonly UIMenu menu;
        private readonly float spacing;
        private readonly float padding;

        private readonly List<UIObject> children = new();

        private float currentX;
        private float tallestHeight = 0f;

        private UIHorizontalLayout(UIMenu menu, float spacing, float padding)
        {
            this.menu = menu;
            this.spacing = spacing;
            this.padding = padding;

            currentX = padding; // left padding
        }

        public static UIHorizontalLayout AttachTo(UIMenu menu, float spacing = 8f, float padding = 10f)
        {
            return new UIHorizontalLayout(menu, spacing, padding);
        }

        public void Add(UIObject obj)
        {
            obj.SetParent(menu.Rect);

            // Set anchors for horizontal layout
            obj.Rect.anchorMin = new Vector2(0f, 0.5f);
            obj.Rect.anchorMax = new Vector2(0f, 0.5f);
            obj.Rect.pivot = new Vector2(0f, 0.5f);

            // Place object
            obj.SetPosition(new Vector2(currentX, 0));

            currentX += obj.Rect.sizeDelta.x + spacing;

            // Track tallest child for height fitting
            if (obj.Rect.sizeDelta.y > tallestHeight)
                tallestHeight = obj.Rect.sizeDelta.y;

            children.Add(obj);
        }

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

        /// <summary>
        /// Adjust menu width to fit children + padding
        /// </summary>
        public void FitMenuWidth()
        {
            float totalWidth = currentX + padding - spacing;
            menu.SetSize(new Vector2(totalWidth, menu.Rect.sizeDelta.y));
        }

        /// <summary>
        /// Adjust menu height to tallest child
        /// </summary>
        public void FitMenuHeight()
        {
            float newHeight = tallestHeight + padding * 2;
            menu.SetSize(new Vector2(menu.Rect.sizeDelta.x, newHeight));
        }
    }
}
