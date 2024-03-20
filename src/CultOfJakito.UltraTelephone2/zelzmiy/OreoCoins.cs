using System;
using System.Collections.Generic;
using System.Text;
using Configgy;
using CultOfJakito.UltraTelephone2.Chaos;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using HarmonyLib;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.zelzmiy
{
    [HarmonyPatch]
    [RegisterChaosEffect]
    internal class OreoCoins : ChaosEffect
    {
        [Configgable("zelzmiy", "Oreo Coins")]
        private static ConfigToggle s_enabled = new(true);


        private static GameObject s_oreo;
        private static GameObject s_oreoSplash;

        public override void BeginEffect(UniRandom random)
        {
            s_oreo = UltraTelephoneTwo.Instance.ZelzmiyBundle.LoadAsset<GameObject>("Oreo");
            s_oreoSplash = UltraTelephoneTwo.Instance.ZelzmiyBundle.LoadAsset<GameObject>("Oreo Splash");
        }

        public override int GetEffectCost() => 1;

        [HarmonyPatch(typeof(Coin), "Start"), HarmonyPostfix]
        private static void ChangeCoinToOreo(Coin __instance)
        {
            if (!s_enabled.Value || !s_oreoSplash)
                return;

            __instance.flash = s_oreoSplash;

            Renderer renderer = __instance.gameObject.GetComponent<Renderer>();
            if (renderer)
            {
                renderer.enabled = false;
            }

            Instantiate(s_oreo, __instance.transform);
        }
    }
}
