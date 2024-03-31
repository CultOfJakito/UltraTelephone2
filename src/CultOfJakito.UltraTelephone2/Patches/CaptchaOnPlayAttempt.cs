using System;
using System.Collections.Generic;
using System.Text;
using Configgy;
using CultOfJakito.UltraTelephone2.Fun.Captcha;
using CultOfJakito.UltraTelephone2.Util;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace CultOfJakito.UltraTelephone2.Patches
{
    [HarmonyPatch]
    public static class CaptchaOnPlayAttempt
    {

        [Configgable("Patches", "Captcha Play Button")]
        private static ConfigToggle s_enabled = new ConfigToggle(true);

        [HarmonyPatch(typeof(CanvasController), nameof(CanvasController.Awake)), HarmonyPostfix]
        private static void OnCanvasAwake(CanvasController __instance)
        {
            if (!s_enabled.Value)
                return;

            if (SceneHelper.CurrentScene != "Main Menu")
                return;

            Button playButton = __instance.transform.GetComponentsInChildren<Button>().FirstOrDefault(x=>x.name == "Continue" && x.transform.HasParentalPath("Main Menu (1)/Continue"));
            if (playButton == null)
                return;

            Button.ButtonClickedEvent bce = playButton.onClick;
            playButton.m_OnClick = new Button.ButtonClickedEvent();

            // Add a captcha check to the play button :3
            playButton.onClick.AddListener(() =>
            {
                CaptchaManager.ShowCaptcha((r) =>
                {
                    if(r)
                    {
                        bce.Invoke();
                        playButton.m_OnClick = bce; // Restore the original event
                    }
                    else
                    {
                        CaptchaManager.ShowCaptcha();
                    }
                });
            });
        }
    }
}
