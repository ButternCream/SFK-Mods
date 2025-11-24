using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SFKUILib
{
    public class UIOverlay
    {
        public UIObject Root { get; private set; }
        public UIVerticalLayout Menu { get; private set; }

        public Image FullscreenBackground { get; private set; } // optional
        public Image PanelBackground { get; private set; }      // optional

        public UICloseButton CloseButton { get; private set; }

        private UIOverlay() { }

        public void Hide() => Root.Active = false;
        public void Show() => Root.Active = true;
        public void Destroy() => GameObject.Destroy(Root.GameObject);

        public static UIOverlay Create(
            string name,
            Vector2 menuPos,
            Transform canvasOverride = null,
            float spacing = 5f,
            float padding = 5f,

            OverlayBackgroundMode bgMode = OverlayBackgroundMode.None,
            Color? bgColor = null,
            Vector2? panelSize = null,
            bool bgBlocksRaycasts = true
        )
        {
            var overlay = new UIOverlay();

            // Find canvas
            var canvas = canvasOverride;
            if (canvas == null)
            {
                var canvasGo = GameObject.Find("Canvas");
                if (canvasGo == null)
                    canvasGo = UnityEngine.Object.FindObjectOfType<Canvas>()?.gameObject;
                canvas = canvasGo.transform;
            }

            // Root full-screen container
            overlay.Root = new UIObject();
            overlay.Root.GameObject = new GameObject(name);
            overlay.Root.Rect = overlay.Root.GameObject.AddComponent<RectTransform>();
            overlay.Root.SetParent(canvas);

            var rootRect = overlay.Root.Rect;
            rootRect.anchorMin = new Vector2(0, 0);
            rootRect.anchorMax = new Vector2(1, 1);
            rootRect.offsetMin = Vector2.zero;
            rootRect.offsetMax = Vector2.zero;

            // ----- Fullscreen background -----
            if (bgMode == OverlayBackgroundMode.Fullscreen)
            {
                overlay.FullscreenBackground =
                    CreateBackgroundImage("FullscreenBG", overlay.Root.Rect, bgColor, stretchFullscreen: true);

                if (bgBlocksRaycasts)
                    overlay.FullscreenBackground.raycastTarget = true;
            }

            // Menu vertical layout anchored relative to center
            overlay.Menu = UIVerticalLayout.Create(overlay.Root.Rect, Vector2.zero, spacing, padding);

            // ----- Panel fixed background -----
            if (bgMode == OverlayBackgroundMode.PanelFixed)
            {
                var size = panelSize ?? new Vector2(520f, 420f);

                overlay.PanelBackground =
                    CreateBackgroundImage("PanelBG", overlay.Root.Rect, bgColor, stretchFullscreen: false);

                var bgRT = overlay.PanelBackground.rectTransform;

                // Center-anchored panel, positioned by menuPos
                bgRT.anchorMin = new Vector2(0.5f, 0.5f);
                bgRT.anchorMax = new Vector2(0.5f, 0.5f);
                bgRT.pivot = new Vector2(0.5f, 0.5f);
                bgRT.sizeDelta = size;
                bgRT.anchoredPosition = menuPos;   // ✅ menuPos now moves the panel
                bgRT.SetAsFirstSibling();

                // Parent the menu to the panel
                overlay.Menu.container.Rect.SetParent(bgRT, false);

                // Top-left menu inside panel with padding inset
                var menuRT = overlay.Menu.container.Rect;
                menuRT.anchorMin = new Vector2(0f, 1f);
                menuRT.anchorMax = new Vector2(0f, 1f);
                menuRT.pivot = new Vector2(0f, 1f);

                // inset from panel top-left
                menuRT.anchoredPosition = new Vector2(padding, -padding);

                // give menu a width to align text against
                menuRT.sizeDelta = new Vector2(size.x, 0f);
            }

            var closeSprite = Util.GetCloseSprite();
            if (closeSprite != null)
            {
                // Parent to panel if PanelFixed, otherwise fullscreen root
                Transform closeParent =
                    (bgMode == OverlayBackgroundMode.PanelFixed && overlay.PanelBackground != null)
                        ? overlay.PanelBackground.rectTransform
                        : overlay.Root.Rect;

                Vector2 offset = new Vector2(-8f, -8f);

                overlay.CloseButton = UICloseButton.Create(
                    parent: closeParent,
                    closeSprite: closeSprite,
                    offsetFromTopRight: offset,
                    enableShake: true
                );

                overlay.CloseButton.onClick(() => overlay.Hide());
            }

            return overlay;
        }

        private static Image CreateBackgroundImage(
            string goName,
            Transform parent,
            Color? color,
            bool stretchFullscreen)
        {
            var bgGO = new GameObject(goName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            var rt = bgGO.GetComponent<RectTransform>();
            rt.SetParent(parent, false);

            if (stretchFullscreen)
            {
                rt.anchorMin = new Vector2(0, 0);
                rt.anchorMax = new Vector2(1, 1);
                rt.offsetMin = Vector2.zero;
                rt.offsetMax = Vector2.zero;
            }

            var img = bgGO.GetComponent<Image>();
            img.sprite = null;
            img.type = Image.Type.Simple;
            img.color = color ?? new Color(0, 0, 0, 0.6f);
            img.raycastTarget = false;

            return img;
        }

        // ----- Fluent helpers -----

        public UIOverlay AddHeader(string text, int size = 32, Color? color = null)
        {
            var header = UIText.Create(text, Menu.container.Rect, Vector2.zero, size, color: color);
            Menu.Add(header);
            return this;
        }

        public UIOverlay AddText(string text, int size = 18, Color? color = null, bool wrap = false)
        {
            var row = UIText.Create(text, Menu.container.Rect, Vector2.zero, size, alignment: TMPro.TextAlignmentOptions.Left, color: color, wrap: wrap);
            Menu.Add(row);
            return this;
        }

        public UIOverlay AddButton(string label, UnityAction onClick, Vector2? size = null)
        {
            var btn = UIButton.Create(
                label,
                Menu.container.Rect,
                size ?? UIButton.STANDARD_SIZE,
                Vector2.zero,
                enableShake: true);

            btn.onClick(onClick); // adapt if needed
            Menu.Add(btn);
            return this;
        }
    }
}
