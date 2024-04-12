using Configgy;
using CultOfJakito.UltraTelephone2.Assets;
using HarmonyLib;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Patches
{
    [HarmonyPatch]
    public static class HoneyBunGrenades
    {
        [Configgable("Patches", "Honey Bun Grenades")]
        private static ConfigToggle s_enabled = new ConfigToggle(true);

        [HarmonyPatch(typeof(Grenade), nameof(Grenade.Start)), HarmonyPostfix]
        private static void OnStart(Grenade __instance)
        {
            if (!s_enabled.Value)
                return;

            if (__instance.rocket)
                return;

            MeshRenderer grenadeRenderer = __instance.GetComponent<MeshRenderer>();
            if (grenadeRenderer == null)
                return;

            grenadeRenderer.enabled = false;

            GameObject honeyBun = GameObject.Instantiate(HydraAssets.HoneyBunModel, __instance.transform);
            honeyBun.transform.localPosition = new Vector3(0,0, 0.332f);
            honeyBun.transform.localRotation = Quaternion.identity;
            honeyBun.transform.localScale = Vector3.one * 1.72f;
        }
    }
}
