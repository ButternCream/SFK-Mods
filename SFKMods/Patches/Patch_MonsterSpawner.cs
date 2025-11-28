using HarmonyLib;
using SuperFantasyKingdom.Spawner;

namespace SFKMod.Patches
{
    [HarmonyPatch(typeof(MonsterSpawner), nameof(MonsterSpawner.IncreaseMonsterAmountMultiplier))]
    public static class Patch_Bonfire
    {
        static void Prefix(ref float by)
        {
            float percentage = 1000f;
            by = percentage / 100;

            Plugin.Logger.LogInfo($"Increased difficulty by {percentage}%");
        }
    }
}
