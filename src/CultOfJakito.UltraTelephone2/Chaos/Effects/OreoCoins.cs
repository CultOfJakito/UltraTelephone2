using Configgy;
using CultOfJakito.UltraTelephone2.Assets;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using HarmonyLib;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Chaos.Effects
{
    [HarmonyPatch]
    [RegisterChaosEffect]
    internal class OreoCoins : ChaosEffect
    {
        [Configgable("Chaos/Effects", "Oreo Coins")]
        private static ConfigToggle s_enabled = new(true);

        private static bool s_effectActive;

        private static GameObject s_oreo;
        private static GameObject s_oreoSplash;

        public override void BeginEffect(UniRandom random)
        {
            s_oreo = UT2Assets.GetAsset<GameObject>("Assets/Telephone 2/Oreo Coins/Oreo.prefab");
            s_oreoSplash = UT2Assets.GetAsset<GameObject>("Assets/Telephone 2/Oreo Coins/Oreo Splash.prefab");
            s_effectActive = true;
        }

        public override bool CanBeginEffect(ChaosSessionContext ctx) => s_enabled.Value && base.CanBeginEffect(ctx);

        public override int GetEffectCost() => 1;

        [HarmonyPatch(typeof(Coin), "Start"), HarmonyPostfix]
        private static void ChangeCoinToOreo(Coin __instance)
        {
            if (!s_enabled.Value || !s_effectActive)
                return;

            __instance.flash = s_oreoSplash;

            Renderer renderer = __instance.gameObject.GetComponent<Renderer>();
            if (renderer)
            {
                renderer.enabled = false;
            }

            Instantiate(s_oreo, __instance.transform);
        }

        protected override void OnDestroy() => s_effectActive = false;

    }
}
