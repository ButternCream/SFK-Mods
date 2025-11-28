using HarmonyLib;
using SuperFantasyKingdom;
using SuperFantasyKingdom.Buildings;
using SuperFantasyKingdom.UI;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NewBuildingTest
{
    // All this just to get another button
    // Essentially replicating AddBuildingButtons
    [HarmonyPatch(typeof(UIOverlayBuildingSelection), "AddBuildingButtons")]
    static class Patch_AddBuildingButtons
    {
        static void Postfix(UIOverlayBuildingSelection __instance, Vector3 pos, BuildingSize size)
        {
            var my = Plugin.BigWood;
            if (my == null) return;

            var myCity = my.GetComponent<BuildingCity>();
            if (myCity == null) return;

            // only add for matching size/category and standard gating
            if (size != myCity.GetBuildingSize()) return;
            if (myCity.GetBuildingCategory() == BuildingCategory.None) return;
            if (!CityManager.Instance.CanBuild(myCity.GetBuildingType())) return;

            var cost = myCity.GetCost();
            if (cost != null && cost.Length > 0 && cost[0].amount < 0) return;

            // parent/category bucket
            Transform parent = __instance.GetParent(myCity.GetBuildingCategory(), __instance.transform);
            GameObject btn = Object.Instantiate(__instance.buttonTemplate, parent);

            // ----- TEXT (force your custom text here no matter what localization does)
            var nameTf = btn.transform.Find("TextName");
            var descTf = btn.transform.Find("TextDescription");

            if (nameTf != null)
            {
                var nameLoc = nameTf.GetComponent<UnityEngine.Localization.Components.LocalizeStringEvent>();
                if (nameLoc != null) nameLoc.enabled = false;

                var nameTMP = nameTf.GetComponent<TextMeshProUGUI>();
                if (nameTMP != null) nameTMP.text = "Massive Wood";
            }

            if (descTf != null)
            {
                var descLoc = descTf.GetComponent<UnityEngine.Localization.Components.LocalizeStringEvent>();
                if (descLoc != null) descLoc.enabled = false;

                var descTMP = descTf.GetComponent<TextMeshProUGUI>();
                if (descTMP != null) descTMP.text = "You get x10 wood!";
            }

            // ----- ICON / SIZE / COSTS (same as vanilla)
            var imageTf = btn.transform.Find("Image");
            int num = (size == BuildingSize.Small || size == BuildingSize.WorldBig) ? 32 : 24;

            if (imageTf != null)
            {
                imageTf.GetComponent<RectTransform>().sizeDelta =
                    myCity.icon.bounds.size * num;
                imageTf.GetComponent<Image>().sprite = myCity.icon;
            }

            btn.SetActive(true);
            UIOverlayBuildingSelection.DisplayCosts(cost, btn);

            // ----- CLICK: set flag so build uses your prototype
            var button = btn.GetComponent<Button>();
            if (!myCity.CanBeBuilt())
            {
                button.interactable = false;
                btn.GetComponent<CanvasGroup>().alpha = 0.7f;
            }
            else
            {
                button.onClick.AddListener(() =>
                {
                    Plugin.IsPlacingBigWood = true;
                    __instance.ButtonClicked(pos, myCity);
                    Plugin.IsPlacingBigWood = false;
                });

                GamepadManager.Instance.SetGameObjectToSelect(btn, false);
            }
        }
    }
    // We cloned "WorldWood" so replace it with our new prefab
    [HarmonyPatch(typeof(CityManager), "GetBuilding", new[] { typeof(BuildingType) })]
    static class Patch_CityManager_GetBuilding
    {
        static void Postfix(BuildingType type, ref BuildingCity __result)
        {
            if (!Plugin.IsPlacingBigWood) return;
            if (type != BuildingType.WorldWood) return;
            if (Plugin.BigWood == null) return;

            __result = Plugin.BigWood;
        }
    }
    // Override the rewards and ignore the Check/Fix functions which keep resetting it 
    [HarmonyPatch(typeof(Reward), nameof(Reward.Check))]
    static class Patch_Reward_Check_BigWood
    {
        static bool Prefix(Reward __instance, bool real, ref List<int> __result)
        {
            // only during your special build click
            if (!Plugin.IsPlacingBigWood) return true;

            var forced = new List<RewardOption>
            {
                new RewardOptionResource(ResourceType.Wood, 10) { probability = 100 }
            };

            __instance.m_Reward = forced;
            __instance.m_DidCheck = true;

            __result = null;   // caller ignores list for Spawn anyway
            return false;      // skip original Check (and thus Fix)
        }
    }

}