using Configgy;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using HarmonyLib;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Chaos
{
    [HarmonyPatch]
    [RegisterChaosEffect]
    public class InconsistentSharpshooterReflect : ChaosEffect
    {
        [Configgable("Chaos/Effects/Inconsistent Sharpshooter", "Inconsistent Sharpshooter")]
        private static ConfigToggle s_enabled = new ConfigToggle(true);

        [Configgable("Chaos/Effects/Inconsistent Sharpshooter", "Random Reflect Chance")]
        private static FloatSlider s_randomReflectChance = new FloatSlider(0.6f, 0f, 1f);

        [Configgable("Chaos/Effects/Inconsistent Sharpshooter", "Random Reflect Variance")]
        private static FloatSlider s_randomReflectRange = new FloatSlider(0.3f, 0f, 1f);

        private static bool s_effectActive = false;

        private static UniRandom s_rng;

        public override void BeginEffect(UniRandom random)
        {
            s_rng = random;
            s_effectActive = true;
        }

        public override bool CanBeginEffect(ChaosSessionContext ctx) => s_enabled.Value && base.CanBeginEffect(ctx);

        public override int GetEffectCost()
        {
            return 3;
        }

        public override void Dispose()
        {
            s_effectActive = false;
            base.Dispose();
        }


        [HarmonyPatch(typeof(RevolverBeam), "RicochetAimAssist"), HarmonyPrefix]
        public static bool OnRicochetAimAssist(RevolverBeam __instance)
        {
            if (!s_enabled.Value || !s_effectActive)
                return true;

            if(!s_rng.Chance(s_randomReflectChance.Value))
                return true;

            Vector3? surfaceNormal = null;
            Vector3 castPoint = __instance.transform.position + __instance.transform.forward * 0.05f;

            //Get the surface normal since we know the beam is on a surface and we can just raycast backwards
            if (Physics.Raycast(castPoint, -__instance.transform.forward, out RaycastHit hit, 0.07f, LayerMaskDefaults.Get(LMD.Environment)))
            {
                surfaceNormal = hit.normal;
            }

            //Set the forward to the surface normal
            if (surfaceNormal != null)
                __instance.transform.forward = surfaceNormal.Value;

            //restock the ricochet amount :3
            __instance.ricochetAmount = 3;

            Vector3 dir = s_rng.InsideUnitSphere() * s_randomReflectRange.Value;
            Vector3 newForward = (__instance.transform.forward+dir).normalized;
            __instance.transform.forward = newForward;
            return false;
        }
    }
}
