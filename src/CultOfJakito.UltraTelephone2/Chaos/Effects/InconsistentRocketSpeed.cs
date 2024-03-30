using Configgy;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using HarmonyLib;

namespace CultOfJakito.UltraTelephone2.Chaos.Effects
{
    [HarmonyPatch]
    [RegisterChaosEffect]
    public class InconsistentRocketSpeed : ChaosEffect
    {
        [Configgable("Chaos/Effects/Inconsistent Rocket Speed", "Inconsistent Rocket Speed")]
        private static ConfigToggle s_enabled = new ConfigToggle(true);

        [Configgable("Chaos/Effects/Inconsistent Rocket Speed", "Speed Range")]
        private static ConfigInputField<float> s_speedRangeMin = new ConfigInputField<float>(0.1f, (v) =>
        {
            return v <= s_speedRangeMax.Value;
        });

        [Configgable("Chaos/Effects/Inconsistent Rocket Speed", "Speed Range")]
        private static ConfigInputField<float> s_speedRangeMax = new ConfigInputField<float>(100f, (v) =>
        {
            return v >= s_speedRangeMin.Value;
        });


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
            return 5;
        }

        public override void Dispose()
        {
            s_effectActive = false;
            base.Dispose();
        }

        [HarmonyPatch(typeof(Grenade), nameof(Grenade.Start)), HarmonyPostfix]
        private static void OnStart(Grenade __instance)
        {
            if (!s_effectActive || !s_enabled.Value)
                return;

            if (!__instance.rocket)
                return;

            __instance.rocketSpeed = s_rng.Range(s_speedRangeMin.Value, s_speedRangeMax.Value);
        }
    }
}
