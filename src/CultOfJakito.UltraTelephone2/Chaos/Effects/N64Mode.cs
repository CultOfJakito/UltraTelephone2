using Configgy;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using HarmonyLib;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Chaos.Effects
{
    [RegisterChaosEffect]
    [HarmonyPatch]
    public class N64Mode : ChaosEffect
    {
        [Configgable("Chaos/Effects", "N64 Mode")]
        private static ConfigToggle s_enabled = new ConfigToggle(true);
        public override void BeginEffect(UniRandom random)
        {
            s_effectActive = true;
            Shader.EnableKeyword("Fooled");
        }

        private static bool s_effectActive = false;

        [HarmonyPatch(typeof(PostProcessV2_Handler), nameof(PostProcessV2_Handler.Fooled)), HarmonyPrefix]
        private static bool OnFooled(PostProcessV2_Handler __instance)
        {
            if (!s_effectActive || !s_enabled.Value)
                return true;

            return false;
        }

        public override bool CanBeginEffect(ChaosSessionContext ctx) => s_enabled.Value && base.CanBeginEffect(ctx);

        public override int GetEffectCost()
        {
            return 1;
        }

        protected override void OnDestroy()
        {
            s_effectActive = false;
            Shader.DisableKeyword("Fooled");
        }
    }
}
