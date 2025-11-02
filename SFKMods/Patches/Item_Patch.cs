// File: ModItemPatches.cs
using HarmonyLib;
using SFKMod.Mods;
using SuperFantasyKingdom;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace ModItems
{
    [HarmonyPatch]
    public static class ModItemPatches
    {
        // Per-statsRoot cache; auto-released when statsRoot is GC'd
        static readonly ConditionalWeakTable<object, Dictionary<string, Stat>> s_statIndexCache
            = new ConditionalWeakTable<object, Dictionary<string, Stat>>();

        static StatModifierType ToGameType(ModValueType t) => t switch
        {
            ModValueType.Flat => StatModifierType.Flat,
            ModValueType.PercentAdd => StatModifierType.PercentAdd,
            ModValueType.PercentMult => StatModifierType.PercentMult,
            _ => StatModifierType.Flat
        };

        [HarmonyPatch(typeof(Item), nameof(Item.Apply))]
        static class Item_Apply_Custom
        {
            [HarmonyPrefix]
            static bool Prefix(Item __instance, object target, bool force)
            {
                var beh = (__instance as Component)?.GetComponent<ModItemBehaviour>();
                if (beh?.Def == null) return true; // not ours → vanilla

                Plugin.Logger.LogInfo($"[ModItems] Apply {beh.Def.id} to {(target as UnityEngine.Object)?.name}");

                var entity = target as Entity;
                var statsRoot = entity?.GetStats();
                if (statsRoot == null)
                {
                    Plugin.Logger.LogWarning("[ModItems] Apply: no stats root");
                    return false;
                }

                // Build or fetch the cached index once per statsRoot
                var index = GetOrBuildStatIndex(statsRoot);

                foreach (var m in beh.Def.statMods)
                {
                    if (!index.TryGetValue(m.statPath, out var stat) || stat == null)
                    {
                        Plugin.Logger.LogWarning($"[ModItems] Stat '{m.statPath}' not found");
                        continue;
                    }

                    var data = new StatModifierData
                    {
                        value = m.value,
                        type = ToGameType(m.valueType),
                        priority = 0,
                        // use -1 to survive RemoveAllModifiers(force:false)
                        origin = m.origin == 0 ? -1 : m.origin
                    };
                    stat.AddModifier(data);
                }

                // end/consume like vanilla
                __instance.End();
                return false; // skip vanilla attack.Trigger path
            }
        }

        // ---------------- Indexing helpers (public-only, built once) ----------------

        static Dictionary<string, Stat> GetOrBuildStatIndex(object statsRoot)
        {
            if (!s_statIndexCache.TryGetValue(statsRoot, out var map))
            {
                map = BuildStatIndex(statsRoot);
                s_statIndexCache.Add(statsRoot, map);
            }
            return map;
        }

        static Dictionary<string, Stat> BuildStatIndex(object root)
        {
            var map = new Dictionary<string, Stat>(StringComparer.Ordinal);
            Visit(root, map);
            return map;

            static void Visit(object obj, Dictionary<string, Stat> map)
            {
                if (obj == null) return;

                const BindingFlags BF = BindingFlags.Instance | BindingFlags.Public; // Publicizer exposes what you need
                var t = obj.GetType();

                // Fields first (fast, predictable)
                foreach (var f in t.GetFields(BF))
                {
                    var v = f.GetValue(obj);
                    if (v is Stat s)
                        map[f.Name] = s;
                    else if (IsPlain(v))
                        Visit(v, map);
                }

                // Optional: public, non-indexed properties
                foreach (var p in t.GetProperties(BF))
                {
                    if (!p.CanRead || p.GetIndexParameters().Length != 0) continue;
                    object v; try { v = p.GetValue(obj, null); } catch { continue; }
                    if (v is Stat s)
                        map[p.Name] = s;
                    else if (IsPlain(v))
                        Visit(v, map);
                }
            }

            static bool IsPlain(object v)
            {
                if (v == null) return false;
                var vt = v.GetType();
                return !vt.IsPrimitive && vt != typeof(string) && !typeof(UnityEngine.Object).IsAssignableFrom(vt);
            }
        }

        // ---------------- UI overrides for our items ----------------

        [HarmonyPatch(typeof(Item), nameof(Item.GetTitle))]
        [HarmonyPostfix]
        static void Item_GetTitle(Item __instance, ref string __result)
        {
            var beh = (__instance as Component)?.GetComponent<ModItemBehaviour>();
            if (beh?.Def != null && !string.IsNullOrEmpty(beh.Def.title))
                __result = beh.Def.title;
        }

        [HarmonyPatch(typeof(Item), nameof(Item.GetDescription))]
        [HarmonyPostfix]
        static void Item_GetDescription(Item __instance, ref string __result)
        {
            var beh = (__instance as Component)?.GetComponent<ModItemBehaviour>();
            if (beh?.Def != null && !string.IsNullOrEmpty(beh.Def.description))
                __result = beh.Def.description;
        }

        [HarmonyPatch(typeof(Item), nameof(Item.GetIcon))]
        [HarmonyPostfix]
        static void Item_GetIcon(Item __instance, ref Sprite __result)
        {
            var beh = (__instance as Component)?.GetComponent<ModItemBehaviour>();
            if (beh?.Def != null && beh.Def.icon != null)
                __result = beh.Def.icon;
        }

        [HarmonyPatch(typeof(Item), nameof(Item.GetCost))]
        [HarmonyPostfix]
        static void Item_GetCost(Item __instance, ref int __result)
        {
            var beh = (__instance as Component)?.GetComponent<ModItemBehaviour>();
            if (beh?.Def != null)
                __result = beh.Def.cost;
        }
    }
}
