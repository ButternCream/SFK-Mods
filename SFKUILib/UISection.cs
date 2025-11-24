using UnityEngine;

namespace SFKUILib
{
    public class UISection
    {
        public UIObject Root { get; private set; }
        public UIVerticalLayout Layout { get; private set; }

        private UISection() { }

        public static UISection Create(Transform parent, string name = "Section", float spacing = 2f, float padding = 0f)
        {
            var sec = new UISection();
            sec.Root = new UIObject();
            sec.Root.GameObject = new GameObject(name);
            sec.Root.Rect = sec.Root.GameObject.AddComponent<RectTransform>();
            sec.Root.SetParent(parent);

            sec.Layout = UIVerticalLayout.AttachTo(sec.Root, spacing, padding);
            return sec;
        }
    }
}
