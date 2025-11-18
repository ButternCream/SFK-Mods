using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;

namespace SFKMod
{
    public sealed class AssetManager
    {
        public static TMP_FontAsset[] Fonts;
        public static Sprite[] Sprites;
        public static AssetManager _instance;
        public static bool loaded;

        private AssetManager() 
        {
        }

        public static AssetManager Instance { 
            get { 
                if (_instance == null)
                {
                    _instance = new AssetManager();

                }
                if (!loaded)
                {
                    _instance.LoadAll();
                }
                return _instance;
            } 
        }

        public void LoadAll(bool debug = false)
        {
            Fonts = Resources.FindObjectsOfTypeAll<TMP_FontAsset>();
            Sprites = Resources.FindObjectsOfTypeAll<Sprite>();
            if (debug)
            {
                Plugin.Logger.LogInfo($"Found {Fonts.Length} fonts.");
                Plugin.Logger.LogInfo($"Found {Sprites.Length} sprites.");
            }
            loaded = true;
        }

        public Sprite GetSpriteByName(string name, bool exact = true) 
        { 
            return exact ? Sprites.Where(sprite => sprite.name == name).FirstOrDefault() : Sprites.Where(sprite => sprite.name.Contains(name)).FirstOrDefault();
        }

        public TMP_FontAsset GetFontByName(string name, bool exact = true) 
        {

            return exact ? Fonts.Where(font => font.name == name).FirstOrDefault() : Fonts.Where(font => font.name.Contains(name)).FirstOrDefault();    
        }
    }
}
