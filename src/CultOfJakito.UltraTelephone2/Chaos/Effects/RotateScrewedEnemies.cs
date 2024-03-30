using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;
using Configgy;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using HarmonyLib;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Chaos.Effects
{
    [RegisterChaosEffect]
    [HarmonyPatch]
    internal class RotateScrewedEnemies : ChaosEffect
    {

        [Configgable("Chaos/Effects", "Rotate with Screwdriver")]
        private static ConfigToggle s_enabled = new ConfigToggle(true);

        private static bool s_effectActive = false;

        public override void BeginEffect(UniRandom random)
        {
            s_effectActive = true;
        }

        public override int GetEffectCost() => 1;

        [HarmonyPatch(typeof(Harpoon), nameof(Harpoon.Update))]
        [HarmonyPostfix]
        private static void UnRotateDrill(Harpoon __instance)
        {
            if (!s_enabled.Value || !s_effectActive)
                return;

            // i tried transpiling to just stop calling the rotate method but it didn't work i haet transpilers so im doing this srry
            // this is pretty bad i admit
            bool flag = !__instance.stopped && !__instance.punched && __instance.rb.velocity.magnitude > 1f;
            if(!flag)
            {
                if (__instance.drilling)
                {
                    __instance.transform.Rotate(Vector3.forward, -14400f * Time.deltaTime);
                }
            }
        }
    }
}
