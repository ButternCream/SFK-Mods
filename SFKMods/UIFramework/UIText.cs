using UnityEngine;
using TMPro;

namespace UI
{
    public class UIText : UIObject
    {
        public TextMeshProUGUI Label;

        public static UIText Create(string text, Transform parent, Vector2 pos, int size = 24, Color? color = null)
        {
            var uiText = new UIText();

            uiText.GameObject = new GameObject("UIText");
            uiText.Rect = uiText.GameObject.AddComponent<RectTransform>();
            uiText.SetParent(parent);

            uiText.SetPosition(pos);

            uiText.Label = uiText.GameObject.AddComponent<TextMeshProUGUI>();
            uiText.Label.text = text;
            uiText.Label.font = Util.GetFont();
            uiText.Label.fontSize = size;
            uiText.Label.color = color ?? Color.white;

            uiText.Label.alignment = TextAlignmentOptions.Center;

            return uiText;
        }
    }
}
