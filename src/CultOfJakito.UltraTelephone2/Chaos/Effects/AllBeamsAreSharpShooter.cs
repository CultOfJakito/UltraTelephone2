using Configgy;
using CultOfJakito.UltraTelephone2.Chaos;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using HarmonyLib;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Hydra
{
    [HarmonyPatch]
    [RegisterChaosEffect]
    public class AllBeamsAreSharpshooter : ChaosEffect
    {
        [Configgable("Chaos/Effects/All Beams Sharpshooter", "All Beams Are Sharpshooter")]
        private static ConfigToggle s_enabled = new ConfigToggle(true);

        [Configgable("Chaos/Effects/All Beams Sharpshooter", "Chance To Be Reflective")]
        private static FloatSlider s_reflectChance = new FloatSlider(0.6f, 0f, 1f);

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


        [HarmonyPatch(typeof(RevolverBeam), "Start"), HarmonyPrefix]
        public static void OnStart(RevolverBeam __instance)
        {
            if (!s_enabled.Value || !s_effectActive)
                return;

            if (!s_rng.Chance(s_reflectChance.Value))
                return;

            //Teehee
            __instance.ricochetAmount = 1;
        }
    }
}
