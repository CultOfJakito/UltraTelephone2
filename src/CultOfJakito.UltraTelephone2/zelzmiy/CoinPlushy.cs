using System;
using System.Collections.Generic;
using System.Text;
using Configgy;
using CultOfJakito.UltraTelephone2.Assets;
using CultOfJakito.UltraTelephone2.Chaos;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using HarmonyLib;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.zelzmiy
{
    [RegisterChaosEffect]
    internal class CoinPlushy : ChaosEffect
    {

        [Configgable("Chaos/Effects", "Coin Plushies")]
        private static ConfigToggle s_enabled = new(true);

        private static bool s_effectActive;

        private static List<GameObject> _plushiePrefabs = new()
        {
            UT2Assets.ZelzmiyBundle.LoadAsset<GameObject>("estrogen burger.prefab"),
        };

        private static int _randomPlushieIndex;

        public override void BeginEffect(UniRandom random)
        {
            _randomPlushieIndex = random.Next(0, 1);
            s_effectActive = true;
        }

        public override int GetEffectCost() => 1;

        [HarmonyPatch(typeof(Coin), "Start"), HarmonyPostfix]
        private static void ChangeCoinToPlushie(Coin __instance)
        {
            if (!s_enabled.Value || !s_effectActive)
                return;

            GameObject plushie = _plushiePrefabs[_randomPlushieIndex];

            GameObject plush = Instantiate(plushie,__instance.transform.position,__instance.transform.rotation);
            plush.GetComponent<Rigidbody>().AddForce(__instance.GetComponent<Rigidbody>().velocity, ForceMode.VelocityChange);
            
            Destroy(__instance.gameObject);
        }
    }
}
