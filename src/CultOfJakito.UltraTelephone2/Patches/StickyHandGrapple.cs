using System;
using System.Collections.Generic;
using System.Text;
using Configgy;
using CultOfJakito.UltraTelephone2.Assets;
using HarmonyLib;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Patches
{
    [HarmonyPatch]
    public static class StickyHandGrapple
    {
        [Configgable("Patches", "Sticky Hand Grapple")]
        private static ConfigToggle s_enabled = new ConfigToggle(true);

        [HarmonyPatch(typeof(HookArm), nameof(HookArm.Start)), HarmonyPostfix]
        private static void OnStart(HookArm __instance)
        {
            if (!s_enabled.Value)
                return;

            //change the rope material
            __instance.lr.material = HydraAssets.StickyHandMaterial;

            //Add custom sound/particle
            __instance.clinkSparks = HydraAssets.StickyHandClink;

            //disable the renderer
            __instance.hookModel.GetComponent<MeshRenderer>().enabled = false;

            Transform hookPoint = __instance.hook;

            //create the sticky hand model
            GameObject stickyHandObj = GameObject.Instantiate(HydraAssets.StickyHandModel, hookPoint);
            stickyHandObj.layer = 13;
            stickyHandObj.transform.localPosition = new Vector3(0.0085f, 0.059f, -0.0016f);
            stickyHandObj.transform.localRotation = Quaternion.Euler(-180f, -1.52f, 5.081985f);
            stickyHandObj.transform.localScale = Vector3.one * 3.770377f;

        }

    }
}
