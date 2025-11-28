using HarmonyLib;
using SuperFantasyKingdom;

namespace No_Breaks.Patches
{
    [HarmonyPatch(typeof(DaytimeManager), nameof(DaytimeManager.IsWorkTime))]
    static class Patch_Work
    {
        static void Postfix(DaytimeManager __instance, ref bool __result)
        {
            // Balance it so they dont just keep working when you wait to enter tavern
            __result = __instance.m_DaytimePhase != DaytimeManager.DaytimePhase.Night;
        }
    }
}
