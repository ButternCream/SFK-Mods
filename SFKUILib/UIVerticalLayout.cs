using System.Collections.Generic;
using TMPro;
using UnityEngine;

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

        public static UIVerticalLayout Create(Transform parentCanvas, Vector2 anchoredPos, float childSpacing = 2f, float childPadding = 4f)
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

            // ✅ Force all children into top-left coordinate space of the container
            rt.anchorMin = new Vector2(0f, 1f);
            rt.anchorMax = new Vector2(0f, 1f);
            rt.pivot = new Vector2(0f, 1f);

            float height = 0f;

            var tmp = obj.GameObject.GetComponent<TextMeshProUGUI>();
            if (tmp != null)
            {
                tmp.ForceMeshUpdate();
                height = tmp.preferredHeight;
            }
            else
            {
                var layoutElement = obj.GameObject.GetComponent<UnityEngine.UI.LayoutElement>();
                if (layoutElement != null)
                {
                    if (layoutElement.preferredHeight > 0) height = layoutElement.preferredHeight;
                    else if (layoutElement.minHeight > 0) height = layoutElement.minHeight;
                }
            }

            if (height <= 0)
                height = rt.sizeDelta.y;

            float dynamicSpacing = spacing + height * 0.15f;

            // ✅ X padding from left, Y cursor from top
            obj.SetPosition(new Vector2(padding, currentY));

            currentY -= height + dynamicSpacing;
            children.Add(obj);
        }
    }
}
