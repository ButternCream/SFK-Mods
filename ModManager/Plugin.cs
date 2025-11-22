using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Logging;
using HarmonyLib;
using SFKUILib;
using SuperFantasyKingdom.TitleScreen;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ModManager
{
    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    [BepInDependency("com.sfk.uilib")]
    public class Plugin : BaseUnityPlugin
    {
        internal static new ManualLogSource Logger;

        public const string PLUGIN_GUID = "com.sfk.modmanager";
        public const string PLUGIN_NAME = "SFK ModManager";
        public const string PLUGIN_VERSION = "1.0.0";

        public static Dictionary<string, PluginInfo> loadedPlugins = Chainloader.PluginInfos;

        bool triggered = false;
        private void Awake()
        {
            // Plugin startup logic
            Logger = base.Logger;
            Logger.LogInfo($"Plugin {PLUGIN_GUID} is loaded!");

            Harmony harmony = new Harmony(PLUGIN_GUID);
            harmony.PatchAll();
        }


        public static void CreateButtonModsListOverlay()
        {
            var canvasGo = GameObject.Find("Canvas");
            var vLayout = UIVerticalLayout.Create(canvasGo.transform, new Vector2(-700, 100));
            for (int i = 0; i < loadedPlugins.Count; i++)
            {
                var btn = UIButton.Create(loadedPlugins.ElementAt(i).Value.Metadata.Name, vLayout.container.Rect, UIButton.STANDARD_SIZE, Vector2.zero, enableShake: true);
                vLayout.Add(btn);
            }
        }

        public static void CreateSmallTextModsListOverlay()
        {
            var canvasGo = GameObject.Find("Canvas");
            var vLayout = UIVerticalLayout.Create(canvasGo.transform, new Vector2(-200, 100));
            for (int i = 0; i < loadedPlugins.Count; i++)
            {
                var text = UIText.Create(loadedPlugins.ElementAt(i).Value.Metadata.Name, vLayout.container.Rect, Vector2.zero, size: 16);
                vLayout.Add(text);
            }
        }

        public static void CreateBigTextModsListOverlay()
        {
            var canvasGo = GameObject.Find("Canvas");
            var vLayout = UIVerticalLayout.Create(canvasGo.transform, new Vector2(-400, 100));
            for (int i = 0; i < loadedPlugins.Count; i++)
            {
                var text = UIText.Create(loadedPlugins.ElementAt(i).Value.Metadata.Name, vLayout.container.Rect, Vector2.zero, size: 32);
                vLayout.Add(text);
            }
        }

        void Update()
        {
            if (!triggered && TitleScreenManager.Instance.menu.activeSelf)
            {
                CreateButtonModsListOverlay();
                CreateSmallTextModsListOverlay();
                CreateBigTextModsListOverlay();
                triggered = true;
            }

        }
    }
}
