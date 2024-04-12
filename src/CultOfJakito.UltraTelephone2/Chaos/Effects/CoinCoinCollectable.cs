using Configgy;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using CultOfJakito.UltraTelephone2.Fun.Coin;
using CultOfJakito.UltraTelephone2.Util;
using HarmonyLib;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Chaos.Effects
{
    //stolen from Zel :3c thanks
    [RegisterChaosEffect]
    [HarmonyPatch]
    public class CoinCoinCollectable : ChaosEffect
    {
        [Configgable("Chaos/Effects", "Coin Throw Coin Collectable")]
        private static ConfigToggle s_enabled = new(true);

        private static bool s_effectActive;

        private static UniRandom s_rng;

        public override void BeginEffect(UniRandom random)
        {
            s_rng = random;
            s_effectActive = true;
        }

        public override bool CanBeginEffect(ChaosSessionContext ctx) => s_enabled.Value && base.CanBeginEffect(ctx);

        public override int GetEffectCost() => 1;

        [HarmonyPatch(typeof(Coin), nameof(Coin.Start)), HarmonyPostfix]
        private static void ChangeCoinToCollectable(Coin __instance)
        {
            if (!s_enabled.Value || !s_effectActive)
                return;

            // 10% coin chance
            if (s_rng.Chance(0.9f))
                return;

            Vector3 forceVector = CameraController.Instance.transform.forward * 20 + Vector3.up * 15f + (NewMovement.Instance.ridingRocket ?
                NewMovement.Instance.ridingRocket.rb.velocity :
                NewMovement.Instance.rb.velocity);

            CoinCollectable coin = CoinCollectable.ThrowRandomCoin(CameraController.Instance.transform.position, forceVector, s_rng);
            coin.isCollectable = false;

            // Re-enable the coin bool after a second so the player can collect the coin
            coin.DoAfterTime(0.7f, () => coin.isCollectable = true);

            Destroy(__instance.gameObject);
        }

        protected override void OnDestroy() => s_effectActive = false;
    }
}
