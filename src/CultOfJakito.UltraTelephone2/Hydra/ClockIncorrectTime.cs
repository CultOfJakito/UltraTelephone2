using System;
using System.Collections.Generic;
using System.Text;
using Configgy;
using CultOfJakito.UltraTelephone2.Chaos;
using HarmonyLib;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Hydra
{

    [HarmonyPatch(typeof(Clock))]
    public static class ClockIncorrectTime 
    {
        [Configgable("Patches", "Broken Clock")]
        private static ConfigToggle s_enabled = new(true);

        static int? s_offset = null;

        [HarmonyPatch(typeof(Clock), nameof(Clock.Update)), HarmonyPrefix]
        public static bool OnUpdate(Clock __instance)
        {
            if(!s_enabled.Value)
                return true;

            if (!s_offset.HasValue)
            {
                s_offset = UltraTelephoneTwo.Instance.Random.Range(-12*60,12*60);
            }

            DateTime clockOffset = DateTime.Now.AddMinutes(s_offset.Value);
            float hour = (float)clockOffset.Hour;
            float minute = (float)clockOffset.Minute;
            float second = (float)clockOffset.Second;
            __instance.hour.localRotation = Quaternion.Euler(0f, (hour % 12f / 12f + minute / 1440f) * 360f, 0f);
            __instance.minute.localRotation = Quaternion.Euler(0f, (minute / 60f + second / 3600f) * 360f, 0f);
            return false;
        }
    }
}
