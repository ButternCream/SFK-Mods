// File: ModItem_ApplyAtManager.cs
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
    [HarmonyPatch(typeof(ItemManager), nameof(ItemManager.Apply))]
    static class ItemManager_Apply_ModItems
    {
        // Per-statsRoot cache: automatically releases when statsRoot is GC'd
        static readonly ConditionalWeakTable<object, Dictionary<string, Stat>> s_statIndexCache
            = new ConditionalWeakTable<object, Dictionary<string, Stat>>();

        [HarmonyPrefix]
        static bool Prefix(string itemIdentifier, Entity entity)
        {
            Plugin.Logger.LogInfo($"[ModItems] Manager.Apply {itemIdentifier} -> {(entity as UnityEngine.Object)?.name}");

            // Not ours? Let vanilla run.
            if (string.IsNullOrEmpty(itemIdentifier) || !itemIdentifier.StartsWith("mod:", StringComparison.OrdinalIgnoreCase))
                return true;

            if (entity == null)
            {
                Plugin.Logger.LogWarning("[ModItems] Apply: entity null");
                return false;
            }

            if (!ModItemRegistry.TryGet(itemIdentifier, out var def) || def == null)
            {
                Plugin.Logger.LogWarning($"[ModItems] Apply: no def for '{itemIdentifier}'");
                return false; // skip vanilla, nothing to do
            }

            var statsRoot = entity.GetStats();
            if (statsRoot == null)
            {
                Plugin.Logger.LogWarning("[ModItems] Apply: stats root null");
                return false;
            }

            // Build or get cached index for this statsRoot
            var index = GetOrBuildStatIndex(statsRoot);

            foreach (var m in def.statMods)
            {
                if (!index.TryGetValue(m.statPath, out var stat) || stat == null)
                {
                    Plugin.Logger.LogWarning($"[ModItems] Apply: stat '{m.statPath}' not found");
                    continue;
                }

                // IMPORTANT: use origin = -1 to survive the game's RemoveAllModifiers(force:false)
                var data = new StatModifierData
                {
                    value = m.value,
                    type = ToGameType(m.valueType),
                    priority = 0,
                    origin = m.origin == 0 ? -1 : m.origin
                };
                stat.AddModifier(data);
            }

            Plugin.Logger.LogInfo($"[ModItems] Applied {def.id} to {(entity as UnityEngine.Object)?.name}");

            // We handled it; skip vanilla ItemManager.Apply (which would load/trigger Item attack path).
            return false;
        }

        static StatModifierType ToGameType(ModValueType t) => t switch
        {
            ModValueType.Flat => StatModifierType.Flat,
            ModValueType.PercentAdd => StatModifierType.PercentAdd,
            ModValueType.PercentMult => StatModifierType.PercentMult,
            _ => StatModifierType.Flat
        };

        // ---------- Indexing (built once per statsRoot) ----------

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

                // Public instance members only — publicizer exposes what used to be private
                const BindingFlags BF = BindingFlags.Instance | BindingFlags.Public;
                var t = obj.GetType();

                // Fields first (fast, predictable)
                foreach (var f in t.GetFields(BF))
                {
                    var v = f.GetValue(obj);
                    if (v is Stat s)
                    {
                        map[f.Name] = s; // last write wins if duplicate names appear
                    }
                    else if (IsPlain(v))
                    {
                        Visit(v, map);
                    }
                }

                // Optional: public, non-indexed properties (skip if you want max safety/perf)
                foreach (var p in t.GetProperties(BF))
                {
                    if (!p.CanRead || p.GetIndexParameters().Length != 0) continue;

                    object v;
                    try { v = p.GetValue(obj, null); }
                    catch { continue; }

                    if (v is Stat s)
                    {
                        map[p.Name] = s;
                    }
                    else if (IsPlain(v))
                    {
                        Visit(v, map);
                    }
                }
            }

            static bool IsPlain(object v)
            {
                if (v == null) return false;
                var vt = v.GetType();
                return !vt.IsPrimitive
                       && vt != typeof(string)
                       && !typeof(UnityEngine.Object).IsAssignableFrom(vt);
            }
        }
    }
}
