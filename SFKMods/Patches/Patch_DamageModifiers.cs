using HarmonyLib;
using SuperFantasyKingdom;

namespace SFKMod.Patches
{
    [HarmonyPatch(typeof(StatContainer), nameof(StatContainer.ApplyTargetDependingModifier))]
    class Patch_ApplyTargetDependingModifier
    {
        static void Prefix(Entity target, TargetDependingStatModifier targetDependingStatModifier)
        {
            if (targetDependingStatModifier.statType == StatTypes.Damage)
                Plugin.Logger.LogInfo($"TDSM applied: amt={targetDependingStatModifier.amount} type={targetDependingStatModifier.modifierType}");
        }
    }
}
