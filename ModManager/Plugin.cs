using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Logging;
using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using SFKUILib;

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

            var profile = GameObject.Find("Canvas/Left/Menu/Profiles").GetComponent<RectTransform>();
            var exit = GameObject.Find("Canvas/Left/Menu/Exit").GetComponent<RectTransform>();

            var spacing = exit ? profile.anchoredPosition.y - exit.anchoredPosition.y : -60.0f;

            var btn = UIButton.Create("Mods", transformGo, UIButton.STANDARD_SIZE, Vector2.zero, rounded: true, enableShake: true);
            // Copy Exit's layout rules
            var rect = btn.Rect;
            rect.anchorMin = profile.anchorMin;
            rect.anchorMax = profile.anchorMax;
            rect.pivot = profile.pivot;

            rect.sizeDelta = profile.sizeDelta;
            rect.anchoredPosition = new Vector2(profile.anchoredPosition.x, profile.anchoredPosition.y + spacing);

            // Insert after Exit
            btn.Rect.SetSiblingIndex(profile.GetSiblingIndex() + 1);

            btn.onClick(() => CreateModsListOverlay());
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
