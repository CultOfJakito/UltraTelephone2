using System;
using System.Collections.Generic;
using System.Text;
using Configgy;
using CultOfJakito.UltraTelephone2.Util;
using HarmonyLib;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Patches
{
    [HarmonyPatch]
    public static class WeirdCrushers
    {
        [Configgable("Patches", "Weird Crushers")]
        private static ConfigToggle s_enabled = new ConfigToggle(true);

        [HarmonyPatch(typeof(Piston), nameof(Piston.Awake)), HarmonyPostfix]
        private static void OnAwake(Piston __instance)
        {
            if (!s_enabled.Value)
                return;

            UniRandom random = new UniRandom(new SeedBuilder()
                .WithGlobalSeed()
                .WithSceneName()
                .WithSeed(__instance.transform.position));

            //Minimum return time needed to be able to extend fully
            const float minReturnTime = 0.18f;

            //Fast return or slow return
            __instance.returnTime = (random.Bool()) ? random.Range(minReturnTime, minReturnTime*1.5f) : random.Range(2f, 4f);
            __instance.attackTime = (random.Bool()) ? random.Range(0.05f,0.5f) : random.Range(3f, 6f);

            //random chance to be harmless...
            if(random.Chance(0.05f))
            {
                if(__instance.dzone is BoxCollider)
                {
                    DeathZone dz = __instance.dzone.GetComponent<DeathZone>();
                    if(dz != null)
                    {
                        UnityEngine.Object.Destroy(dz);
                    }
                }

            }else if (random.Chance(0.05f)) //Random chance to be super evil....
            {
                __instance.returnTime = minReturnTime;
                __instance.attackTime = 0f;
            }
        }
    }
}
