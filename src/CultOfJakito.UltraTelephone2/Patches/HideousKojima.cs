using System;
using System.Collections.Generic;
using System.Text;
using Configgy;
using CultOfJakito.UltraTelephone2.Assets;
using HarmonyLib;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Patches
{
    //[HarmonyPatch]
    public static class HideousKojima
    {
        //[Configgable("Patches", "Hideous Kojima")]
        private static ConfigToggle s_enabled = new ConfigToggle(true);

        //[HarmonyPatch(typeof(Mass), nameof(Mass.Start)), HarmonyPostfix]
        public static void CreateTheBeast(Mass __instance)
        {
            if (!s_enabled.Value)
                return;

            //This is disabled bc I edited the wrong texture... oops
            return;

            EnemyIdentifier eid = __instance.GetComponent<EnemyIdentifier>();

            //sanity check for like custom enemies or something
            if (eid == null || eid.enemyType != EnemyType.HideousMass)
                return;

            SkinnedMeshRenderer smr = __instance.GetComponentInChildren<SkinnedMeshRenderer>(true);
            smr.material.mainTexture = HydraAssets.HideousKojimaTexture;


            //Instance it so we don't change the original material
            __instance.enrageMaterial = new Material(__instance.enrageMaterial);
            __instance.enrageMaterial.mainTexture = HydraAssets.HideousKojimaTexture;

        }
    }
}
