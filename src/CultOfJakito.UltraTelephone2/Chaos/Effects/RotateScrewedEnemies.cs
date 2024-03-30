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

        [Configgable("Chaos/Effects/Rotate Screwdriver", "Rotate with Screwdriver")]
        private static ConfigToggle s_enabled = new ConfigToggle(true);

        [Configgable("Chaos/Effects/Rotate Screwdriver", "Rotation Amount")]
        private static ConfigInputField<int> s_rotationAmount = new(14400);

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
                    Vector3 vec = new();
                    vec = Vector3.forward;
                    __instance.target.eid.transform.Rotate(vec, s_rotationAmount.Value * Time.deltaTime);
                }
            }
        }
        private void OnDestroy() => s_effectActive = false;
    }
}
