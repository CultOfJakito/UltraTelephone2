using System;
using System.Collections.Generic;
using System.Text;
using Configgy;
using HarmonyLib;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Hydra
{
    [HarmonyPatch]
    public static class ZeroGravityCoins
    {
        [Configgable("Hydra/Patches", "Zero-G Coins")]
        private static ConfigToggle s_enabled = new ConfigToggle(true);

        [HarmonyPatch(typeof(Coin), "Start"), HarmonyPostfix]
        public static void OnStart(Coin __instance)
        {
            if (!s_enabled.Value)
                return;

            if(__instance.TryGetComponent<Rigidbody>(out Rigidbody rb))
                rb.useGravity = false;
        }
    }
}
