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

        [Configgable("Chaos/Effects/Inconsistent Rocket Speed", "Slow Rocket Speed")]
        private static ConfigInputField<float> s_slowRocketSpeed = new ConfigInputField<float>(0.1f, (v) =>
        {
            return v <= s_fastRocketSpeed.Value;
        });

        [Configgable("Chaos/Effects/Inconsistent Rocket Speed", "Fast Rocket Speed")]
        private static ConfigInputField<float> s_fastRocketSpeed = new ConfigInputField<float>(100f, (v) =>
        {
            return v >= s_slowRocketSpeed.Value;
        });

        [Configgable("Chaos/Effects/Inconsistent Rocket Speed", "Speed Variance")]
        private static ConfigInputField<float> s_speedVarianceMultiplier = new ConfigInputField<float>(10f);

        private static bool s_effectActive = false;
        private static UniRandom s_rng;

        public override void BeginEffect(UniRandom random)
        {
            s_rng = random;
            s_effectActive = true;
        }

        public override bool CanBeginEffect(ChaosSessionContext ctx) => s_enabled.Value && base.CanBeginEffect(ctx);

        public override int GetEffectCost() => 5;

        protected override void OnDestroy() => s_effectActive = false;


        [HarmonyPatch(typeof(Grenade), nameof(Grenade.Start)), HarmonyPostfix]
        private static void OnStart(Grenade __instance)
        {
            if (!s_effectActive || !s_enabled.Value)
                return;

            if (!__instance.rocket)
                return;

            float variance = s_rng.Float() * s_speedVarianceMultiplier.Value;
            float speed = 0f;

            //Fast rocket
            if (s_rng.Bool())
                speed = s_fastRocketSpeed.Value + variance; 
            else //Slow rocket
                speed = s_slowRocketSpeed.Value + variance;

            __instance.rocketSpeed = speed;
        }
    }
}
