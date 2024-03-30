using System;
using System.Collections.Generic;
using System.Text;
using Configgy;
using CultOfJakito.UltraTelephone2.Assets;
using CultOfJakito.UltraTelephone2.Util;
using HarmonyLib;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Patches
{
    [HarmonyPatch]
    public static class AltMarksmanFoghorn
    {
        [Configgable("Patches", "Alt Marksman Foghorn")]
        private static ConfigToggle s_enabled = new ConfigToggle(true);

        [HarmonyPatch(typeof(Revolver), nameof(Revolver.OnEnable)), HarmonyPostfix]
        public static void OnStart(Revolver __instance)
        {
            if (!s_enabled.Value)
                return;

            if (!__instance.altVersion || __instance.gunVariation != (int)WeaponVariant.GreenVariant)
                return;

            //I dont want to mess with the normal audio source, so create a new one
            AudioSource src = Locator.LocateComponentInChildren<AudioSource>(__instance.transform, "FOGHORN");

            if(src == null)
            {
                GameObject srcObject = new GameObject("FOGHORN");
                srcObject.transform.SetParent(__instance.transform);
                srcObject.transform.localPosition = Vector3.zero;

                src = srcObject.AddComponent<AudioSource>();
                src.ConfigureForUltrakill();
                src.clip = HydraAssets.FogHorn;
                src.loop = false;
                src.volume = 1f;
                src.spatialBlend = 1f;
            }

            src.Play();
        }
    }
}
