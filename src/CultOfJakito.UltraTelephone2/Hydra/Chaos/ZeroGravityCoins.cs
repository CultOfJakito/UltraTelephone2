using Configgy;
using CultOfJakito.UltraTelephone2.Chaos;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using HarmonyLib;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Hydra
{
    [HarmonyPatch]
    [RegisterChaosEffect]
    public class ZeroGravityCoins : ChaosEffect
    {
        [Configgable("Hydra/Chaos", "Zero-G Coins")]
        private static ConfigToggle s_enabled = new ConfigToggle(true);

        [HarmonyPatch(typeof(Coin), "Start"), HarmonyPostfix]
        public static void OnStart(Coin __instance)
        {
            if (!s_enabled.Value || !s_effectActive)
                return;

            if(__instance.TryGetComponent<Rigidbody>(out Rigidbody rb))
                rb.useGravity = false;
        }

        private static bool s_effectActive = false;

        public override void BeginEffect(UniRandom random)
        {
            s_effectActive = true;
        }

        public bool CanBeginEffect(ChaosSessionContext ctx) =>  s_enabled.Value && base.CanBeginEffect(ctx);

        public override int GetEffectCost()
        {
            return 1;
        }

        public override void Dispose()
        {
            s_effectActive = false;
            base.Dispose();
        }
    }
}
