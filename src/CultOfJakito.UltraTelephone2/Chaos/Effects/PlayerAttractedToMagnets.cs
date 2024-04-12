using Configgy;
using HarmonyLib;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Chaos.Effects
{
    // [RegisterChaosEffect]
    public class PlayerAttractedToMagnets : ChaosEffect
    {
        private static bool s_effectActive;

        [Configgable("Chaos/Effects", "Magnets Pull Player")]
        private static ConfigToggle s_enabled = new(true);

        [HarmonyPatch(typeof(Magnet), nameof(Magnet.OnTriggerEnter)), HarmonyPostfix]
        public static void OnTriggerEnter(Magnet __instance, Collider other)
        {
            if (!s_effectActive || !s_enabled.Value)
                return;

            if (!other.CompareTag("Player"))
                return;

            __instance.affectedRbs.Add(NewMovement.Instance.rb);
        }

        [HarmonyPatch(typeof(Magnet), nameof(Magnet.OnTriggerEnter)), HarmonyPostfix]
        public static void OnTriggerExit(Magnet __instance, Collider other)
        {
            if (!s_effectActive || !s_enabled.Value)
                return;

            if (!other.CompareTag("Player"))
                return;

            __instance.affectedRbs.Remove(NewMovement.Instance.rb);
        }

        private static UniRandom s_rng;

        public override void BeginEffect(UniRandom random) => s_effectActive = true;

        public override bool CanBeginEffect(ChaosSessionContext ctx) => s_enabled.Value && base.CanBeginEffect(ctx);
        public override int GetEffectCost() => 1;
        protected override void OnDestroy() => s_effectActive = false;
    }
}
