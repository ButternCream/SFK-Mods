using UnityEngine;
using UnityEngine.UI;

namespace SFKUILib
{
    public class UICloseButton : UIButton
    {
        public static Vector2 DEFAULT_SIZE = new Vector2(26, 26);

        /// <summary>
        /// Creates an icon-only close button anchored top-right in the given parent.
        /// </summary>
        public static UICloseButton Create(
            Transform parent,
            Sprite closeSprite,
            Vector2? size = null,
            Vector2? offsetFromTopRight = null,
            Color? tint = null,
            bool enableShake = true)
        {
            var btn = new UICloseButton();

            btn.GameObject = new GameObject("Close");
            btn.Rect = btn.GameObject.AddComponent<RectTransform>();
            btn.SetParent(parent);

            var rt = btn.Rect;
            var finalSize = size ?? DEFAULT_SIZE;
            var finalOffset = offsetFromTopRight ?? new Vector2(-10, -10);

            // ✅ top-right anchored inside parent
            rt.anchorMin = new Vector2(1f, 1f);
            rt.anchorMax = new Vector2(1f, 1f);
            rt.pivot = new Vector2(1f, 1f);
            rt.sizeDelta = finalSize;
            rt.anchoredPosition = finalOffset;

            // Background Image (icon)
            btn.Background = btn.GameObject.AddComponent<Image>();
            btn.Background.sprite = closeSprite;
            btn.Background.type = Image.Type.Simple;
            btn.Background.preserveAspect = true;
            btn.Background.color = tint ?? Color.white;

            // Button
            btn.Button = btn.GameObject.AddComponent<Button>();
            btn.Button.onClick = new Button.ButtonClickedEvent();

            // Optional shake / game clickable
            if (enableShake)
            {
                btn.Clickable = btn.GameObject.AddComponent<SuperFantasyKingdom.UI.UIClickable>();
                btn.Clickable.buttonThatMustBeEnabledAndInteractable = btn.Button;
            }

            return btn;
        }
    }
}
