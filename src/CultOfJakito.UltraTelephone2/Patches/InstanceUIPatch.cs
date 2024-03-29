using System;
using System.Collections.Generic;
using System.Text;
using CultOfJakito.UltraTelephone2.Assets;
using HarmonyLib;
using UltraTelephone.Hydra;
using UnityEngine;
using UnityEngine.UI;

namespace CultOfJakito.UltraTelephone2
{
    [HarmonyPatch]
    public static class InstanceUIPatch
    {
        [HarmonyPatch(typeof(CanvasController), "Awake"), HarmonyPostfix]
        public static void OnAwake(CanvasController __instance)
        {
            RectTransform rect = __instance.GetComponent<RectTransform>();
            MakeTitleImageUT2(rect);
        }

        //Sets the main title image to ULTRATELEPHONE 2 logo
        private static void MakeTitleImageUT2(RectTransform canvasRect)
        {
            if (SceneHelper.CurrentScene != "Main Menu")
                return;

            GamepadObjectSelector menuComp = canvasRect.GetComponentsInChildren<GamepadObjectSelector>().Where(x => x.name == "Main Menu (1)").FirstOrDefault();
            if (menuComp == null)
                return;

            Image ultrakillImage = menuComp.GetComponentInChildren<Image>();
            ultrakillImage.sprite = HydraAssets.UT2Banner;
        }
    }
}
