using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace LoadCatalogFix
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public static ManualLogSource Log;
        private static Harmony _harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);

        private void Awake()
        {
            Log = Logger;
            Log.LogInfo($"{MyPluginInfo.PLUGIN_GUID} Loaded.");
            _harmony.PatchAll();
        }

        private void OnDestroy()
        {
            _harmony.UnpatchSelf();
        }
    }
}
