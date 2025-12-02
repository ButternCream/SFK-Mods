using HarmonyLib;
using SuperFantasyKingdom;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace LoadCatalogFix.Patches
{
    [HarmonyPatch(typeof(AddressablesManager), nameof(AddressablesManager.LoadCatalog))]
    public static class Patch_LoadCatalog
    {
        [HarmonyPrefix]
        private static bool LoadCatalog_Prefix(AddressablesManager __instance, ref IEnumerator __result)
        {
            __result = FixedLoadCatalog(__instance);
            return false;
        }

        private static IEnumerator FixedLoadCatalog(AddressablesManager self)
        {
            var handlers = new List<AsyncOperationHandle>();

            handlers.Add(Addressables.LoadAssetsAsync<GameObject>("unit", obj =>
            {
                var unit = obj.GetComponent<Unit>();
                unit.SetIdentifier(self.m_UnitsAmount);
                self.m_Units.Add(obj.name, unit);
                if (!self.m_UnitIdentifier.Contains(unit.GetEntityIdentifier()))
                {
                    self.m_UnitIdentifier.Add(unit.GetEntityIdentifier());
                }
                self.m_UnitsAmount++;
            }));

            handlers.Add(Addressables.LoadAssetsAsync<GameObject>("hero", obj =>
            {
                var hero = obj.GetComponent<Hero>();
                self.m_Heroes.Add(obj.name, hero);
                if (!self.m_HeroIdentifier.Contains(hero.GetEntityIdentifier()))
                {
                    self.m_HeroIdentifier.Add(hero.GetEntityIdentifier());
                }
            }));
            handlers.Add(Addressables.LoadAssetsAsync<GameObject>("relic", obj =>
            {
                var relic = obj.GetComponent<Relic>();
                self.m_Relics.Add(obj.name, relic);
                if (!self.m_RelicIdentifier.Contains(relic.GetIdentifier()))
                {
                    self.m_RelicIdentifier.Add(relic.GetIdentifier());
                }
            }));
            handlers.Add(Addressables.LoadAssetsAsync<GameObject>("item", obj =>
            {
                var item = obj.GetComponent<Item>();
                self.m_Items.Add(obj.name, item);
                if (!self.m_ItemIdentifier.Contains(item.GetIdentifier()))
                {
                    self.m_ItemIdentifier.Add(item.GetIdentifier());
                }
            }));
            handlers.Add(Addressables.LoadAssetsAsync<GameObject>("monster", obj =>
            {
                var monster = obj.GetComponent<Monster>();
                self.m_Monster.Add(obj.name, monster);
                if (monster.GetWave() > 0 && !self.m_MonsterIdentifier.Contains(monster.GetEntityIdentifier()))
                {
                    self.m_MonsterIdentifier.Add(monster.GetEntityIdentifier());
                }
            }));
            handlers.Add(Addressables.LoadAssetsAsync<GameObject>("boss", obj =>
            {
                var boss = obj.GetComponent<Boss>();
                self.m_Bosses.Add(obj.name, boss);
            }));
            handlers.Add(Addressables.LoadAssetsAsync<GameObject>("villain", obj =>
            {
                var villain = obj.GetComponent<Villain>();
                self.m_Villains.Add(obj.name, villain);
            }));
            handlers.Add(Addressables.LoadAssetsAsync<Achievement>("achievement", achievement =>
            {
                self.m_Achievements.Add(achievement);
            }));

            foreach (var h in handlers)
                yield return h;

            Plugin.Log.LogInfo("Assets loaded.");
            self.m_IsLoaded = true;
        }
    }
}
