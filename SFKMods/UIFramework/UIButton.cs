using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UIFramework
{
    public class UIButton : UIObject
    {
        public Image Background;
        public TextMeshProUGUI Label;
        public Button Button;
        public SuperFantasyKingdom.UI.UIClickable Clickable;

        public static Vector2 STANDARD_SIZE = new Vector2(250, 42);

        public UIButton onClick(UnityEngine.Events.UnityAction action)
        {
            Button?.onClick.AddListener(action);
            return this;
        }

        public static UIButton Create(
            string text,
            Transform parent,
            Vector2 size,
            Vector2 pos,
            Color? bgColor = null,
            bool rounded = false,
            bool enableShake = false
            )
        {
            var btn = new UIButton();

            // Base object
            btn.GameObject = new GameObject($"UIButton_{text}");
            btn.Rect = btn.GameObject.AddComponent<RectTransform>();
            btn.SetParent(parent);

            btn.SetSize(size);
            btn.SetPosition(pos);

            // Background
            btn.Background = btn.GameObject.AddComponent<Image>();
            btn.Background.color = bgColor ?? new Color(0, 0, 0, 0.6f);

            // Rounded sprite optional
            if (rounded)
            {
                var sprite = Util.GetRoundedSprite();
                if (sprite != null)
                {
                    btn.Background.sprite = sprite;
                    btn.Background.type = Image.Type.Sliced;
                }
            }

            // Button component
            btn.Button = btn.GameObject.AddComponent<Button>();
            btn.Button.onClick = new Button.ButtonClickedEvent();

            if (enableShake)
            {
                btn.Clickable = btn.GameObject.AddComponent<SuperFantasyKingdom.UI.UIClickable>();
                btn.Clickable.buttonThatMustBeEnabledAndInteractable = btn.Button;
            }

            // Create TMP label
            var labelGO = new GameObject("Text");
            labelGO.transform.SetParent(btn.Rect, false);

            btn.Label = labelGO.AddComponent<TextMeshProUGUI>();
            btn.Label.text = text;
            btn.Label.font = Util.GetFont();
            btn.Label.alignment = TextAlignmentOptions.Center;
            btn.Label.color = Color.white;

            btn.Rect.anchorMin = new Vector2(0.5f, 1f);
            btn.Rect.anchorMax = new Vector2(0.5f, 1f);
            btn.Rect.pivot = new Vector2(0.5f, 1f);

            // Stretch to fill parent
            var txtRect = btn.Label.rectTransform;
            txtRect.anchorMin = Vector2.zero;
            txtRect.anchorMax = Vector2.one;
            txtRect.offsetMin = Vector2.zero;
            txtRect.offsetMax = Vector2.zero;

            return btn;
        }
    }
}
