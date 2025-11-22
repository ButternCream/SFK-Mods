using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SFKUILib
{
    public class UIVerticalLayout
    {
        public readonly IUIContainer container;
        private readonly float spacing;
        private readonly float padding;

        private readonly List<UIObject> children = new();

        private float currentY;

        private UIVerticalLayout(IUIContainer container, float spacing, float padding)
        {
            this.container = container;
            this.spacing = spacing;
            this.padding = padding;

            currentY = -padding;
        }

        public static UIVerticalLayout Create(Transform parentCanvas, Vector2 anchoredPos, float childSpacing = 5f, float childPadding = 5f)
        {
            var root = new UIObject();
            root.GameObject = new GameObject("VerticalLayout");
            root.Rect = root.GameObject.AddComponent<RectTransform>();
            root.SetParent(parentCanvas);

            // Set position relative to center of screen
            root.Rect.anchorMin = new Vector2(0.5f, 0.5f);
            root.Rect.anchorMax = new Vector2(0.5f, 0.5f);
            root.SetPosition(anchoredPos);

            UIVerticalLayout layout = AttachTo(root, childSpacing, childPadding);


            return layout;
        }

        public static UIVerticalLayout AttachTo(IUIContainer container, float spacing = 8f, float padding = 10f)
        {
            return new UIVerticalLayout(container, spacing, padding);
        }

        public void Add(UIObject obj)
        {
            obj.SetParent(container.Rect);

            RectTransform rt = obj.Rect;

            float height = 0f;

            // 1. If object has TMP text, use preferredHeight
            var tmp = obj.GameObject.GetComponent<TextMeshProUGUI>();
            if (tmp != null)
            {
                tmp.ForceMeshUpdate();
                height = tmp.preferredHeight;
            }

            // 2. If object has a LayoutElement, use its min or preferred height
            else
            {
                var layoutElement = obj.GameObject.GetComponent<LayoutElement>();
                if (layoutElement != null)
                {
                    if (layoutElement.preferredHeight > 0)
                        height = layoutElement.preferredHeight;
                    else if (layoutElement.minHeight > 0)
                        height = layoutElement.minHeight;
                }
            }

            // 3. Fallback to RectTransform sizeDelta
            if (height <= 0)
                height = rt.sizeDelta.y;

            // 4. Compute spacing dynamically (tweak multiplier)
            float dynamicSpacing = height * 0.15f; // 15% of height by default

            // 5. Position the element
            obj.SetPosition(new Vector2(0, currentY));

            // 6. Move the layout cursor
            currentY -= height + dynamicSpacing;

            children.Add(obj);
        }
    }
}
