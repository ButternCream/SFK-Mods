using BepInEx;
using BepInEx.Logging;
using KingdomLevelDump;
using SuperFantasyKingdom;                 // PrefabsManager, Races, MainManager, etc.
using SuperFantasyKingdom.Buildings;       // BuildingOutpost
using System.Collections;
using System.Linq;
// AddressablesManager / Relic live in SuperFantasyKingdom namespace in your dump,
// adjust using if your publicized DLL puts them elsewhere.

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class KingdomLevelDumpPlugin : BaseUnityPlugin
{
    private static ManualLogSource Log;

    private void Awake()
    {
        Log = Logger;
        Log.LogInfo("[KingdomLevelDump] Loaded.");
        StartCoroutine(WaitAndDump());
    }

    private IEnumerator WaitAndDump()
    {
        // Wait for singletons to exist
        while (PrefabsManager.Instance == null)
            yield return null;

        while (AddressablesManager.Instance == null)
            yield return null;

        // this is normally broken without my LoadCatalogFix patch
        while (!AddressablesManager.Instance.m_IsLoaded)
            yield return null;

        DumpHardcodedRewards();
        DumpOutpostsAllKingdoms();
        DumpRelicsAll();
    }

    private void DumpHardcodedRewards()
    {
        Log.LogInfo("=== Kingdom Level Hardcoded Rewards ===");
        Log.LogInfo("Coins: EVERY level");
        Log.LogInfo("Road unlocks at levels: 5, 10, 15");
        Log.LogInfo("Board unlocks at levels: 6, 12, 18, 24");
        Log.LogInfo("Jail unlocks at levels: 10, 20, 30");
        Log.LogInfo("Challenges unlock at level: 15");
        Log.LogInfo("Super Metal unlock at level: 20");
    }

    private void DumpOutpostsAllKingdoms()
    {
        Log.LogInfo("=== Outposts by Kingdom Level ===");

        DumpOutpostsForRace(Races.Human, "Human");
        DumpOutpostsForRace(Races.Undead, "Undead");
    }

    private void DumpOutpostsForRace(Races race, string label)
    {
        KingdomData kd = PrefabsManager.Instance.GetKingdomData(race);
        if (kd == null)
        {
            Log.LogWarning($"[KingdomLevelDump] KingdomData null for {label}");
            return;
        }

        BuildingOutpost[] outposts = kd.GetOutposts();
        if (outposts == null || outposts.Length == 0)
        {
            Log.LogWarning($"[KingdomLevelDump] No outposts for {label}");
            return;
        }

        var grouped = outposts
            .Where(o => o != null)
            .GroupBy(o => o.GetRequiredKingdomLevel())
            .OrderBy(g => g.Key);

        Log.LogInfo($"--- {label} Outposts ---");
        foreach (var g in grouped)
        {
            string names = string.Join(", ", g.Select(o => o.GetTitle()));
            Log.LogInfo($"Level {g.Key}: {names}");
        }
    }

    private void DumpRelicsAll()
    {
        Log.LogInfo("=== Relics by Kingdom Level ===");

        // Dictionary<string, Relic>
        var allRelics = AddressablesManager.Instance.GetAllRelics();
        if (allRelics == null || allRelics.Count == 0)
        {
            Log.LogWarning("[KingdomLevelDump] No relics found.");
            return;
        }

        var grouped = allRelics.Values
            .Where(r => r != null)
            .GroupBy(r => r.GetRequiredKingdomLevel())
            .OrderBy(g => g.Key);

        foreach (var g in grouped)
        {
            string names = string.Join(", ",
                g.Select(r => $"{r.GetTitle()} [{r.GetKingdom()}]"));

            Log.LogInfo($"Level {g.Key}: {names}");
        }
    }
}
