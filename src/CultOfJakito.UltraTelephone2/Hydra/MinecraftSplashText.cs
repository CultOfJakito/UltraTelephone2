﻿using System;
using System.Collections.Generic;
using System.Text;
using Configgy;
using CultOfJakito.UltraTelephone2.Data;
using CultOfJakito.UltraTelephone2.Util;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace CultOfJakito.UltraTelephone2.Hydra
{
    [HarmonyPatch]
    public static class MinecraftSplashText
    {
        private static List<string> splashPhrases => UT2TextFiles.S_SplashTextsFile.TextList;
        private static string splashTextFilePath => Path.Combine(UT2Paths.DataFolder, "splashes.txt");

        [Configgable("Fun/SplashText", "Enable Splash Text")]
        private static ConfigToggle s_enabled = new ConfigToggle(true);

        [Configgable("Fun/SplashText", "Change Splash Text")]
        private static ConfigButton changeSplashText = new ConfigButton(ChangeSplashText, "Change Splash Text");

        static int seedOffset = 0;

        [HarmonyPatch(typeof(CanvasController), "Awake"), HarmonyPostfix]
        public static void OnAwake(CanvasController __instance)
        {
            if (!s_enabled.Value)
                return;

            RectTransform canvasRect = __instance.GetComponent<RectTransform>();

            //Create TMP text object and
            GamepadObjectSelector menuComp = canvasRect.GetComponentsInChildren<GamepadObjectSelector>().Where(x => x.name == "Main Menu (1)").FirstOrDefault();
            if (menuComp == null)
                return;

            Image ultrakillImage = menuComp.GetComponentInChildren<Image>();

            Text earlyAccessText = ultrakillImage.GetComponentInChildren<Text>();

            if (earlyAccessText == null)
                return;

            splashText = earlyAccessText;
            splashText.fontSize = 24;

            float imageWidth = ultrakillImage.rectTransform.rect.width;

            Vector2 sizeDelta = splashText.rectTransform.sizeDelta;
            sizeDelta.x = 1000f;
            splashText.rectTransform.sizeDelta = sizeDelta;
            splashText.raycastTarget = false;
            splashText.color = Color.yellow;
            splashText.rectTransform.localRotation = Quaternion.Euler(0, 0, UniRandom.Global.Range(-20f,20f));
            splashText.gameObject.AddComponent<SplashTextBouncer>();
            ChangeSplashText();
        }

        private static void ChangeSplashText()
        {
            if (splashText == null)
                return;

            splashText.text = new UniRandom(UltraTelephoneTwo.Instance.Random.Seed ^ seedOffset).SelectRandom(splashPhrases.ToArray());
            ++seedOffset;
        }

        private static Text splashText;

    }

    public class SplashTextBouncer : MonoBehaviour
    {
        [Configgable("Fun/SplashText", "Bounce Speed")]
        private static ConfigInputField<float> bounceSpeed = new ConfigInputField<float>(2f);

        [Configgable("Fun/SplashText", "Bounce Size")]
        private static ConfigInputField<float> bounceSize = new ConfigInputField<float>(0.2f);

        private void Update()
        {
            float t = Mathf.Sin(Time.time * bounceSpeed.Value);
            float scale = 1 + (t * bounceSize.Value);

            transform.localScale = Vector3.one * scale;
        }
    }
}