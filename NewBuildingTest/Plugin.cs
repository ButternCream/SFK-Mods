using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using SuperFantasyKingdom;
using SuperFantasyKingdom.Buildings;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NewBuildingTest
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        internal static new ManualLogSource Logger;
        public static BuildingOutpostReward BigWood;
        public static bool IsPlacingBigWood = false;
        Harmony _harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);

        private void Awake()
        {
            // Plugin startup logic
            Logger = base.Logger;
            Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");

            SceneManager.sceneLoaded += onSceneLoaded;
            _harmony.PatchAll();
        }

        private void WorldBuildingDetails()
        {
            //[Info: New Building Test] WorldWood | WorldBig | Outpost
            //[Info: New Building Test] WorldWheat | WorldBig | Outpost
            //[Info: New Building Test] WorldStone | WorldBig | Outpost
            //[Info: New Building Test] WorldOre | WorldBig | Outpost
            //[Info: New Building Test] WorldHunter | WorldBig | Outpost
            //[Info: New Building Test] WorldShards | WorldBig | Outpost
            //[Info: New Building Test] WorldFaith | WorldBig | Outpost
            //[Info: New Building Test] WorldBarracks | WorldBig | Outpost
            //[Info: New Building Test] WorldArchery | WorldBig | Outpost
            //[Info: New Building Test] WorldMages | WorldBig | Outpost
            //[Info: New Building Test] WorldSummoners | WorldBig | Outpost
            //[Info: New Building Test] WorldDifficulty | WorldBig | Outpost
            //[Info: New Building Test] WorldBanish | WorldBig | Outpost
            //[Info: New Building Test] WorldCoins | WorldBig | Outpost
            //[Info: New Building Test] WorldAppeal | WorldBig | Outpost
            //[Info: New Building Test] WorldRelic | WorldBig | Outpost
            //[Info: New Building Test] WorldUnit | WorldBig | Outpost
            // Debug Stuff
            var allBuilding = RaceManager.Instance.GetBuildings(BuildingSize.WorldBig);
            BuildingCity woodOutpost = null;
            foreach (var building in allBuilding)
            {
                var bCity = building.GetComponent<BuildingCity>();
                var type = bCity.GetBuildingType();
                if (type == BuildingType.WorldWood)
                {
                    woodOutpost = bCity;
                }
                var size = bCity.GetBuildingSize();
                var category = bCity.GetBuildingCategory();
                Logger.LogInfo($"{type} | {size} | {category}");
            }
            if (woodOutpost != null)
            {
                Logger.LogInfo($"{woodOutpost.GetType().FullName}");
            }

            // Clone wood outpost to make big wood
            if (woodOutpost is BuildingOutpostReward r_Wood)
            {
                var clonedGO = Instantiate(r_Wood.gameObject);
                clonedGO.name = "Mod_WorldWood_Big";
                clonedGO.transform.position = new Vector3(99999, 99999, 99999);
                clonedGO.hideFlags = HideFlags.HideAndDontSave;
                DontDestroyOnLoad(clonedGO);

                BigWood = clonedGO.GetComponent<BuildingOutpostReward>();

                Logger.LogInfo("BigWood prototype created.");
            }

        }

        private void onSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == "GameScene")
            {
                WorldBuildingDetails();
            }
        }
    }
}
