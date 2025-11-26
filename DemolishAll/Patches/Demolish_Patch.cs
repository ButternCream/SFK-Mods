using HarmonyLib;
using SuperFantasyKingdom;
using SuperFantasyKingdom.Buildings;
using SuperFantasyKingdom.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace DemolishAll.Patches
{
    // We need to despawn workers for houses
    [HarmonyPatch(typeof(BuildingCity), "Demolish")]
    public static class Patch_HouseDemolish_DespawnWorkers
    {
        static void Postfix(BuildingCity __instance)
        {
            if (__instance == null) return;

            var workers = JobManager.Instance.GetWorker();
            if (workers == null) return;

            foreach (var w in workers.ToList())
            {
                if (w == null) continue;

                if (__instance == w.GetHome())
                {
                    JobManager.Instance.UnregisterWorker(w);
                    Object.Destroy(w.gameObject);
                }
            }
        }
    }

    [HarmonyPatch(typeof(UIBuildingOverlayElementDemolish), "Select")]
    public static class Patch_DemolishSelectGuard
    {
        static bool Prefix(UIBuildingOverlayElementDemolish __instance)
        {
            OverlayManager.Instance.CloseOverlay(false);
            var msg = OverlayManager.Instance.OverlayMessage(true);

            msg.SetCancel("", () => OverlayManager.Instance.Cancel(true));

            string text = "Demolish this building?";
            var loc = __instance.stringMessage;
            if (loc != null) text = loc.GetLocalizedString();

            msg.SetMessage(text);
            msg.SetAccept(new ResourceAmount(ResourceType.Banish, 1),
                          new UnityAction(__instance.Callback));

            return false; // skip original Select
        }
    }

    [HarmonyPatch(typeof(UIBuildingOverlay), nameof(UIBuildingOverlay.Open))]
    public static class Patch_Demolish
    {
        private static UIBuildingOverlayElementDemolish _element;
        private static float Y_OFFSET = 50f;
        static void Prefix(UIBuildingOverlay __instance, bool force)
        {
            var building = __instance.building;
            // Only add to city buildings
            if (building == null || building is not BuildingCity || building is BuildingCastle) return;

            Plugin.Logger.LogInfo("Demolish Prefix");

            List<UIBuildingOverlayElement> buttons = __instance.buttons;
            if (buttons == null)
            {
                buttons = new List<UIBuildingOverlayElement>();
                __instance.buttons = buttons;
            }

            if (buttons.Any(b => b is UIBuildingOverlayElementDemolish))
                return;

            if (_element == null)
            {
                foreach (var overlay in Object.FindObjectsOfType<UIBuildingOverlay>(true))
                {
                    var list = overlay.buttons;
                    if (list == null) continue;

                    _element = list.FirstOrDefault(e => e is UIBuildingOverlayElementDemolish)
                                    as UIBuildingOverlayElementDemolish;
                    if (_element != null) break;
                }
            }

            if (_element != null)
            {
                Plugin.Logger.LogInfo("Adding Demolish (Cloned)");
                var clone = (UIBuildingOverlayElementDemolish)_element.Clone();
                buttons.Add(clone);
            }
            else
            {
                Plugin.Logger.LogInfo("No Element Demolish");
                buttons.Add(new UIBuildingOverlayElementDemolish());
            }

        }

        static void Postfix(UIBuildingOverlay __instance, bool force)
        {
            OverlayManager.Instance.StartCoroutine(MoveNextFrame());
        }

        private static IEnumerator MoveNextFrame()
        {
            yield return null;

            // Find the specific parent container you showed
            var selection = GameObject.Find(
                "Canvas/UICanvasPP/Overlay/SizeSelection/Panel/Selection"
            );

            if (selection == null)
            {
                Plugin.Logger.LogInfo("Selection container not found.");
                yield break;
            }

            // Find newest OverlayElementDemolish(Clone) under Selection
            var demolishRoot = selection.transform
                .Cast<Transform>()
                .Where(t => t.name.StartsWith("OverlayElementDemolish"))
                .LastOrDefault();

            if (demolishRoot == null)
            {
                Plugin.Logger.LogInfo("OverlayElementDemolish(Clone) not found.");
                yield break;
            }

            if (demolishRoot is RectTransform rt)
            {
                var p = rt.anchoredPosition;
                p.y -= Y_OFFSET; // move down
                rt.anchoredPosition = p;

                Plugin.Logger.LogInfo($"Moved {demolishRoot.name} down by {Y_OFFSET}");
            }
            else
            {
                // fallback
                var lp = demolishRoot.localPosition;
                lp.y -= 0.2f;
                demolishRoot.localPosition = lp;
            }
        }
    }
}
