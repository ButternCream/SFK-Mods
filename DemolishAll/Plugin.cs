using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace DemolishAll
{
    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public const string PLUGIN_GUID = "com.sfkbutter.demolishany";
        public const string PLUGIN_NAME = "SFK Demolish Any";
        public const string PLUGIN_VERSION = "1.0.0";
        internal static new ManualLogSource Logger;

        Harmony _harmony = new Harmony(PLUGIN_GUID);

        private void Awake()
        {
            // Plugin startup logic
            Logger = base.Logger;
            Logger.LogInfo($"Plugin {PLUGIN_GUID} is loaded!");

            _harmony.PatchAll();
        }
    }
}
