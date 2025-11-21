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


        public static void CreateModsListOverlay()
        {
            var go = GameObject.Find("Canvas");
            Logger.LogInfo($"{go.name}");
            var transformGo = go.transform;
            int spacing = -35;
            for (int i = 0; i < loadedPlugins.Count; i++)
            {
                var text = UIText.Create(loadedPlugins.ElementAt(i).Value.Metadata.Name, transformGo, new Vector2(-700, 200 + (i + 1) * spacing), 18);
            }
        }

        void Update()
        {
            if (!triggered && TitleScreenManager.Instance.menu.activeSelf)
            {
                CreateModsListOverlay();
                triggered = true;
            }
           
        }
    }
}
