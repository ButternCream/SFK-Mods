using TMPro;
using UnityEngine;

namespace SFKUILib
{
    public class UIText : UIObject
    {
        public TextMeshProUGUI Label;

        public static UIText Create(string text, Transform parent, Vector2 pos, int size = 24, Color? color = null, bool wrap = false)
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
            uiText.Label.enableWordWrapping = wrap;
            uiText.Label.overflowMode = wrap ? uiText.Label.overflowMode : TextOverflowModes.Overflow;

            return uiText;
        }
    }
}
