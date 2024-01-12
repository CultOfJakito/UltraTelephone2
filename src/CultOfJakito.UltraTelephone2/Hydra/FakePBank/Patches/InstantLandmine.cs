using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Configgy;
using HarmonyLib;

namespace CultOfJakito.UltraTelephone2
{
    [HarmonyPatch]
    public static class InstantLandmine
    {
        [Configgable("Hydra/Tweaks", "Realistic Landmines")]
        private static ConfigToggle enabled = new ConfigToggle(true);


        [HarmonyPatch(typeof(Landmine), nameof(Landmine.Activate)), HarmonyPostfix]
        public static void OnActivate(Landmine __instance, bool ___activated)
        {
            if (!enabled.Value)
                return;

            //Reflection is being dumb so idfc cry about it
            __instance.Invoke("Explode", 0f);
        }

    }
}
