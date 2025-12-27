using HarmonyLib;
using SuperFantasyKingdom;
using SuperFantasyKingdom.Buildings;
using SuperFantasyKingdom.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace DemolishAll.Patches
{

    [HarmonyPatch]
    public static class UIOverlayMessage_SetAccept_Patch
    {
        [HarmonyReversePatch]
        [HarmonyPatch(typeof(UIOverlayMessage), "SetAccept",
        new[] { typeof(ResourceAmount), typeof(UnityAction) })]
        public static Button SetAcceptCost(UIOverlayMessage instance, ResourceAmount cost, UnityAction action)
        {
            throw new NotImplementedException();
        }
    }

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
                    UnityEngine.Object.Destroy(w.gameObject);
                }
            }
        }
    }

    [HarmonyPatch(typeof(UIBuildingOverlayElementDemolish), "Select")]
    public static class Patch_DemolishSelectGuard
    {
        static bool Prefix(UIBuildingOverlayElementDemolish __instance)
        {
            var overlayManager = OverlayManager.Instance;
            if (overlayManager == null || __instance == null)
            {
                return false;
            }
            overlayManager.CloseOverlay(false);
            var msg = overlayManager.OverlayMessage(true);

            msg.SetCancel("", () => overlayManager.Cancel(true));

            string text = __instance.stringMessage?.GetLocalizedString()
                  ?? "Demolish this building?";

            msg.SetMessage(text);
            UnityAction action = () =>
            {
                if (__instance != null)
                    __instance.Callback();
            };
            UIOverlayMessage_SetAccept_Patch.SetAcceptCost(msg, new ResourceAmount(ResourceType.Banish, 1), action);

            return false; // skip original Select
        }
    }

    [HarmonyPatch(typeof(UIBuildingOverlay), nameof(UIBuildingOverlay.Open))]
    public static class Patch_Demolish
    {
        private static bool _moveScheduled = false;
        private static float Y_OFFSET = 50f;
        static void Prefix(UIBuildingOverlay __instance, bool force)
        {
            UIBuildingOverlayElementDemolish _element = null;
            var building = __instance.building;
            // Only add to city buildings
            if (building == null || building is not BuildingCity || building is BuildingCastle) return;

            List<UIBuildingOverlayElement> buttons = __instance.buttons;
            if (buttons == null)
            {
                buttons = new List<UIBuildingOverlayElement>();
                __instance.buttons = buttons;
            }

            if (buttons.Any(b => b is UIBuildingOverlayElementDemolish))
                return;


            foreach (var overlay in UnityEngine.Object.FindObjectsOfType<UIBuildingOverlay>(true))
            {
                var list = overlay.buttons;
                if (list == null) continue;

                _element = list.FirstOrDefault(e => e is UIBuildingOverlayElementDemolish)
                                as UIBuildingOverlayElementDemolish;
                if (_element != null) break;
            }


            if (_element != null)
            {
                Plugin.Logger.LogInfo("Found existing demolish");
                var clone = (UIBuildingOverlayElementDemolish)_element.Clone();
                buttons.Add(clone);
            }
            else
            {
                Plugin.Logger.LogInfo("No demolish element found, creating a new one.");
                buttons.Add(new UIBuildingOverlayElementDemolish());
            }

        }

        static void Postfix(UIBuildingOverlay __instance, bool force)
        {
            if (_moveScheduled) return;
            _moveScheduled = true;
            OverlayManager.Instance.StartCoroutine(MoveNextFrame());
        }

        private static IEnumerator MoveNextFrame()
        {
            yield return null;
            _moveScheduled = false;

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
