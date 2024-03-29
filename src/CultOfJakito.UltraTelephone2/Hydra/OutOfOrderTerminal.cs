using System;
using System.Collections.Generic;
using System.Text;
using Configgy;
using CultOfJakito.UltraTelephone2.Assets;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace CultOfJakito.UltraTelephone2.Hydra
{
    [HarmonyPatch]
    public static class OutOfOrderTerminal
    {
        [Configgable("Patches/Out Of Order Terminal", "Out Of Order Terminals")]
        private static ConfigToggle s_enabled = new ConfigToggle(true);

        [Configgable("Patches/Out Of Order Terminal", "Chance For Out Of Order")]
        private static FloatSlider s_chance = new FloatSlider(0.1f, 0f, 1f);

        [HarmonyPatch(typeof(ShopZone), nameof(ShopZone.Start)), HarmonyPostfix]
        public static void OnShopZoneStart(ShopZone __instance)
        {
            if (!s_enabled.Value)
                return;

            int globalSeed = UltraTelephoneTwo.Instance.Random.Seed;
            int positionSeed = UniRandom.StringToSeed(__instance.transform.position.ToString());
            int levelSeed = UniRandom.StringToSeed(SceneHelper.CurrentScene);
            int seed = globalSeed ^ positionSeed ^ levelSeed;

            UniRandom rng = new UniRandom(seed);

            if (!rng.Chance(s_chance.Value))
                return;

            Canvas canvas = __instance.GetComponentInChildren<Canvas>(true);
            canvas.enabled = false;

            GraphicRaycaster raycaster = canvas.GetComponent<GraphicRaycaster>();
            raycaster.enabled = false;

            AudioSource audioSource = __instance.GetComponentInChildren<AudioSource>();
            audioSource.clip = HydraAssets.BrokenShopMusic;
            audioSource.Play();
            GameObject outOfOrderSign = GameObject.Instantiate(HydraAssets.OutOfOrderShopSign, __instance.transform);
        }
    }
}
