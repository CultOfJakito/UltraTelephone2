using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using UltraTelephone.Hydra;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2
{
    [HarmonyPatch]
    public static class InstanceUIPatch
    {
        [HarmonyPatch(typeof(CanvasController), "Awake"), HarmonyPostfix]
        public static void OnAwake(CanvasController __instance)
        {
            RectTransform rect = __instance.GetComponent<RectTransform>();

            rect.gameObject.AddComponent<Jumpscare>();
        }
    }
}
