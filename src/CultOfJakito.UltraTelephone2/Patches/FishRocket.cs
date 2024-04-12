using Configgy;
using CultOfJakito.UltraTelephone2.Assets;
using CultOfJakito.UltraTelephone2.Util;
using HarmonyLib;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Patches
{
    [HarmonyPatch]
    public static class FishRocket
    {
        [Configgable("Patches", "Fish Rocket")]
        private static ConfigToggle s_enabled = new ConfigToggle(true);

        [HarmonyPatch(typeof(Grenade), nameof(Grenade.Start)), HarmonyPostfix]
        private static void OnStart(Grenade __instance)
        {
            if (!s_enabled.Value)
                return;

            if (!__instance.rocket)
                return;

            MeshRenderer rocketRenderer = __instance.transform.LocateComponentInChildren<MeshRenderer>("Model");
            if (rocketRenderer == null)
                return;

            rocketRenderer.enabled = false;


            GameObject fishRocketObject = GameObject.Instantiate(HydraAssets.RocketFishModel, __instance.transform);
            fishRocketObject.transform.localPosition = new Vector3(0, 0, 3.18f);
            fishRocketObject.transform.localRotation = Quaternion.Euler(-89.98f, 0f, 0f);
            fishRocketObject.transform.localScale = Vector3.one * 155.0367f;

            OutdoorsChecker checker = __instance.GetComponentInChildren<OutdoorsChecker>();
            if (checker != null)
            {
                GameObject[] targets = checker.targets;
                GameObject[] newTargets = new GameObject[targets.Length + 1];
                Array.Copy(targets, newTargets, targets.Length);
                newTargets[targets.Length] = fishRocketObject;
                checker.targets = newTargets;
            }
        }
    }
}
