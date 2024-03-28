using System;
using System.Collections.Generic;
using System.Text;
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

            rect.gameObject.AddComponent<Jumpscare>();
            //MakeTitleImageUT2(rect);
        }

        //Sets the Early Access text to ULTRATELEPHONE 2
        private static void MakeTitleImageUT2(RectTransform canvasRect)
        {
            if (SceneHelper.CurrentScene != "Main Menu")
                return;

            //Create TMP text object and
            GamepadObjectSelector menuComp = canvasRect.GetComponentsInChildren<GamepadObjectSelector>().Where(x => x.name == "Main Menu (1)").FirstOrDefault();
            if (menuComp == null)
                return;

            Image ultrakillImage = menuComp.GetComponentInChildren<Image>();
            Text[] texts = ultrakillImage.GetComponentsInChildren<Text>();
            foreach (Text text in texts)
            {
                text.text = $"-- ULTRATELEPHONE 2 --";
            }
        }
    }
}
