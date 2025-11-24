using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SFKUILib
{
    public class UIHorizontalLayout
    {
        private readonly IUIContainer container;
        private readonly float spacing;
        private readonly float padding;

        private readonly List<UIObject> children = new();

        private float currentX;

        private UIHorizontalLayout(IUIContainer container, float spacing, float padding)
        {
            this.container = container;
            this.spacing = spacing;
            this.padding = padding;

            currentX = padding; // left padding
        }

        public static UIHorizontalLayout Create(Transform parentCanvas, Vector2 anchoredPos)
        {
            var root = new UIObject();
            root.GameObject = new GameObject("Horizontal");
            root.Rect = root.GameObject.AddComponent<RectTransform>();
            root.SetParent(parentCanvas);

            // Set position relative to center of screen
            root.Rect.anchorMin = new Vector2(0.5f, 0.5f);
            root.Rect.anchorMax = new Vector2(0.5f, 0.5f);
            root.SetPosition(anchoredPos);

            UIHorizontalLayout layout = AttachTo(root);

            return layout;
        }

        public static UIHorizontalLayout AttachTo(IUIContainer container, float spacing = 8f, float padding = 10f)
        {
            return new UIHorizontalLayout(container, spacing, padding);
        }

        public void Add(UIObject obj)
        {
            obj.SetParent(container.Rect);

            RectTransform rt = obj.Rect;

            float width = 0f;

            // (1) TextMeshProUGUI width
            var tmp = obj.GameObject.GetComponent<TextMeshProUGUI>();
            if (tmp != null)
            {
                tmp.ForceMeshUpdate();
                width = tmp.preferredWidth;
            }
            else
            {
                // (2) LayoutElement width
                var layoutElement = obj.GameObject.GetComponent<LayoutElement>();
                if (layoutElement != null)
                {
                    if (layoutElement.preferredWidth > 0)
                        width = layoutElement.preferredWidth;
                    else if (layoutElement.minWidth > 0)
                        width = layoutElement.minWidth;
                }
            }

            // (3) Fallback
            if (width <= 0)
                width = rt.sizeDelta.x;

            // Dynamic spacing based on width (15% of width is a good default)
            float dynamicSpacing = width * 0.15f;

            // Position object horizontally, centered vertically
            obj.Rect.anchorMin = new Vector2(0f, 0.5f);
            obj.Rect.anchorMax = new Vector2(0f, 0.5f);
            obj.Rect.pivot = new Vector2(0f, 0.5f);

            obj.SetPosition(new Vector2(currentX, 0));

            currentX += width + dynamicSpacing;

            children.Add(obj);
        }
    }
}
