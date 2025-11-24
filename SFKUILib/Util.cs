using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SFKUILib
{
    public static class Util
    {
        // TMP fallback font
        private static TMP_FontAsset cachedFont;

        public static TMP_FontAsset GetFont()
        {
            if (cachedFont != null)
                return cachedFont;

            // 1. Try to load the game's main TMP font explicitly
            cachedFont = Resources.FindObjectsOfTypeAll<TMP_FontAsset>()
                .FirstOrDefault(f => f.name == "Compass 9");

            if (cachedFont != null)
                return cachedFont;

            // 2. As fallback, find any TMP font in scene
            var anyText = Object.FindObjectOfType<TextMeshProUGUI>();
            if (anyText != null)
            {
                cachedFont = anyText.font;
                return cachedFont;
            }

            // 3. Last fallback: load a Resources font called DefaultFont (if exists)
            cachedFont = Resources.Load<TMP_FontAsset>("DefaultFont");

            if (cachedFont == null)
                Debug.LogWarning("[UIFramework] Could not find Compass 9 or any TMP font!");

            return cachedFont;
        }

        // Rounded corner sprite fallback
        private static Sprite cachedRounded;

        public static Sprite GetRoundedSprite()
        {
            if (cachedRounded != null)
                return cachedRounded;

            // Try load from Resources (if game has one)
            cachedRounded = Resources.Load<Sprite>("UI_27");

            return cachedRounded;
        }

        private static Sprite _closeSpriteCached;
        private static GameObject _closePrefabCached;

        public static Sprite GetCloseSprite()
        {
            if (_closeSpriteCached != null) return _closeSpriteCached;

            // Find any active Close in the scene (adjust path/name if needed)
            var closeGO = GameObject.FindObjectsOfType<GameObject>(true)
                .FirstOrDefault(g =>
                    g.name == "Close" &&
                    g.GetComponent<Image>() != null);

            if (closeGO != null)
            {
                _closeSpriteCached = closeGO.GetComponent<Image>().sprite;
                return _closeSpriteCached;
            }

            return null;
        }

        public static GameObject GetClosePrefab()
        {
            if (_closePrefabCached != null) return _closePrefabCached;

            var closeGO = GameObject.FindObjectsOfType<GameObject>(true)
                .FirstOrDefault(g =>
                    g.name == "Close" &&
                    g.GetComponent<Button>() != null);

            if (closeGO != null)
            {
                _closePrefabCached = closeGO;
                return _closePrefabCached;
            }

            return null;
        }
    }
}
