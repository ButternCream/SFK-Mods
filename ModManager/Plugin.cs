using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Logging;
using HarmonyLib;
using SuperFantasyKingdom;
using SuperFantasyKingdom.Buildings;
using SuperFantasyKingdom.UI;
using System.Collections.Generic;
using UnityEngine;
using UI;
using UnityEngine.SceneManagement;

namespace ModManager
{
    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    [BepInDependency("com.sfk.uilib")]
    public class Plugin : BaseUnityPlugin
    {
        internal static new ManualLogSource Logger;

        public const string PLUGIN_GUID = "com.sfk.modmanager";
        public const string PLUGIN_NAME = "SFK ModManager";
        public const string PLUGIN_VERSION = "1.0.1";

        Dictionary<string, PluginInfo> loadedPlugins = Chainloader.PluginInfos;
        private UIMenu modsMenu;

        private void Awake()
        {
            // Plugin startup logic
            Logger = base.Logger;
            Logger.LogInfo($"Plugin {PLUGIN_GUID} is loaded!");

            Harmony harmony = new Harmony(PLUGIN_GUID);
            harmony.PatchAll();

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void CreateModButton()
        {
            var go = GameObject.Find("Canvas/Left/Menu");
            Logger.LogInfo($"{go.name}");
            var transformGo = go.transform;

            var btn = UIButton.Create("Mods", transformGo, UIButton.STANDARD_SIZE, Vector2.zero, rounded: true, enableShake: true);
            btn.Rect.SetSiblingIndex(transformGo.childCount - 1);
            btn.onClick(() =>
            {
                CreateModsListOverlay();
            });
        }

        private void CreateModsListOverlay()
        {
            if (modsMenu == null)
            {
                var mainCanvas = GameObject.Find("Canvas").transform;

                modsMenu = UIMenu.Create(new Vector2(500, 600), new Vector2(-500, 0), mainCanvas, bgColor: Color.blue);
                var row = modsMenu.AddVerticalLayout();
                foreach (var plugin in loadedPlugins)
                {
                    row.AddButton(plugin.Value.Metadata.Name, new Vector2(325, 50), enableShake: true);
                }
            }
            modsMenu.Active = !modsMenu.Active;
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode loadMode)
        {
            if (scene.name == "TitleScene")
            {
                CreateModButton();
            } 
        }

        void Update()
        {
           
        }
    }
}
