using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using SuperFantasyKingdom;
using SuperFantasyKingdom.Spawner;
using UnityEngine;

namespace SFKMod
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {

        public bool m_AddBg = false;
        private bool m_Visible = false;
        private Rect m_WindowRect = new(100, 100, 300, 200);
        internal static new ManualLogSource Logger;
        private ConfigEntry<KeyboardShortcut> m_ToggleKey;
        private ConfigEntry<float> m_CfgWindowX;
        private ConfigEntry<float> m_CfgWindowY;
        private ConfigEntry<float> m_CfgWindowW;
        private ConfigEntry<float> m_CfgWindowH;

        Harmony harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);

        void Awake()
        {
            // Plugin startup logic
            Logger = base.Logger;

            m_ToggleKey = Config.Bind("General", "ToggleKey", new KeyboardShortcut(KeyCode.F1), "Key to show/hide the GUI panel");

            m_CfgWindowX = Config.Bind("Window", "X", m_WindowRect.x, "Window X position");
            m_CfgWindowY = Config.Bind("Window", "Y", m_WindowRect.y, "Window Y position");
            m_CfgWindowW = Config.Bind("Window", "W", m_WindowRect.width, "Window width");
            m_CfgWindowH = Config.Bind("Window", "H", m_WindowRect.height, "Window height");

            m_WindowRect.x = m_CfgWindowX.Value;
            m_WindowRect.y = m_CfgWindowY.Value;
            m_WindowRect.width = m_CfgWindowW.Value;
            m_WindowRect.height = m_CfgWindowH.Value;


            harmony.PatchAll();
            DontDestroyOnLoad(this);
        }



        void Update()
        {
            if (m_ToggleKey.Value.IsDown())
            {
                m_Visible = !m_Visible;
            }
        }

        void OnGUI()
        {
            if (!m_Visible)
            {
                return;
            }
            m_WindowRect = GUILayout.Window(123456, m_WindowRect, DrawWindowContents, "Mod");
        }

        Vector3 ScreenCenter()
        {
            // Convert screen center to world
            var cam = Camera.main;
            if (!cam)
            {
                Debug.LogWarning("No main camera found!");
                return Vector3.zero;
            }

            Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, cam.nearClipPlane + 5f);
            Vector3 worldPos = cam.ScreenToWorldPoint(screenCenter);
            worldPos.z = 0f;

            return worldPos;
        }

        private void DrawWindowContents(int id)
        {
            GUILayout.BeginVertical();

            GUILayout.Label("IMGUI Panel");
            if (GUILayout.Button("Test Resource Spawn"))
            {
                Instances.DroppedShardSpawner.Spawn(ScreenCenter(), 1, ResourceType.Faith);
            }
            if (GUILayout.Button("Spawn Vagabond"))
            {
                Villain vagabondVillain;
                AddressablesManager.Instance.GetAllVillains().TryGetValue("Vagabond", out vagabondVillain);
                if (vagabondVillain != null)
                {
                    Vector3 pos = new Vector3(vagabondVillain.GetSpawnDistance(), 14f, 0f);
                    var m = CombatManager.Instance.SpawnMonster(vagabondVillain, pos);
                    m.Init(MonsterSpawner.Instance.GetCurrentDifficulty() + 1, true);
                    Logger.LogInfo("Spawned Vagabond villain.");
                    return;
                }
            }

            if (GUILayout.Button("Hero Health"))
            {
                Instances.UnitManager.GetHero().m_CurrentBaseHealth = 99999f;
            }

            GUILayout.Space(8);
            GUILayout.Label($"Window position: {Mathf.RoundToInt(m_WindowRect.x)}, {Mathf.RoundToInt(m_WindowRect.y)}");
            GUILayout.Label($"Window size: {Mathf.RoundToInt(m_WindowRect.width)} x {Mathf.RoundToInt(m_WindowRect.height)}");

            if (GUILayout.Button("Close"))
            {
                m_Visible = false;
            }

            GUILayout.EndVertical();
            GUI.DragWindow(new Rect(0, 0, 10000, 20));
        }

        private void OnApplicationQuit()
        {
            SaveWindowRectToConfig();
            harmony.UnpatchSelf();
        }

        private void OnDestroy()
        {
            SaveWindowRectToConfig();
            harmony.UnpatchSelf();
        }

        private void SaveWindowRectToConfig()
        {
            m_CfgWindowH.Value = m_WindowRect.x;
            m_CfgWindowH.Value = m_WindowRect.y;
            m_CfgWindowH.Value = m_WindowRect.width;
            m_CfgWindowH.Value = m_WindowRect.height;

            Config.Save();
        }

    }
}
