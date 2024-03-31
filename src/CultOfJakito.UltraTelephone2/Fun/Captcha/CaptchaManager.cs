using System;
using System.Collections.Generic;
using System.Text;
using Configgy;
using CultOfJakito.UltraTelephone2.Util;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Fun.Captcha
{
    public class CaptchaManager : MonoBehaviour
    {

        [Configgable("Fun", "Captchas Enabled")]
        private static ConfigToggle s_enabled = new ConfigToggle(true);

        public GameObject CaptchaMainContainer;
        public Captcha[] Captchas;

        private static CaptchaManager s_instance;

        private UniRandom rng;

        private void Awake()
        {
            rng = new UniRandom(new SeedBuilder().WithGlobalSeed().WithSceneName().WithSeed(42));
            s_instance = this;
            RectTransform rt = GetComponent<RectTransform>();
            rt.SetSiblingIndex(15);
        }

        private void Start()
        {
            for (int i = 0; i < Captchas.Length; i++)
            {
                if (Captchas[i] == null)
                    continue;

                Captchas[i].SetManager(this);
            }

            for (int i = 0; i < Captchas.Length; i++)
            {
                if (Captchas[i] == null)
                    continue;

                Captchas[i].HideCaptcha();
            }

            CaptchaMainContainer.SetActive(false);
        }

        private Queue<Action<bool>> captchaQueue = new Queue<Action<bool>>();

        private bool showingCaptcha => CaptchaMainContainer.activeSelf;

        private void Update()
        {
            //when a captcha is available, show it
            if(!showingCaptcha && captchaQueue.Count >0)
            {
                s_instance.ShowCaptchaInternal();
            }
        }

        /// <summary>
        /// Shows a fake captcha to the player. treat bool as 50/50 chance of success as some are just random lmao
        /// </summary>
        /// <param name="onComplete">Callback with true if player succeeded or false if they did not.</param>
        public static void ShowCaptcha(Action<bool> onComplete = null)
        {
            if (!s_enabled.Value)
            {
                //If captchas are disabled, just call the callback with true
                onComplete?.Invoke(true);
                return;
            }

            if(s_instance == null)
                s_instance = FindObjectOfType<CaptchaManager>();

            if (s_instance == null)
                return;

            //just make an empty to add to the queue
            onComplete ??= (success) =>
            {
                //By default, show another if they fail :3c
                if(!success)
                    ShowCaptcha();
            };

            s_instance.captchaQueue.Enqueue(onComplete);

            //Show captcha immediately if none are showing
            if(!s_instance.showingCaptcha)
                s_instance.ShowCaptchaInternal();
        }

        private void ShowCaptchaInternal()
        {
            for (int i = 0; i < Captchas.Length; i++)
            {
                if (Captchas[i] == null)
                    continue;

                Captchas[i].HideCaptcha();
            }

            Captcha captcha = rng.SelectRandom(Captchas);
            CaptchaMainContainer.SetActive(true);
            Pauser.Pause(CaptchaMainContainer);
            captcha.ShowCaptcha();
        }

        public void CaptchaDone()
        {
            if(captchaQueue.Count > 0)
                captchaQueue.Dequeue().Invoke(true);

            CaptchaMainContainer.SetActive(false);
            Pauser.Unpause(CaptchaMainContainer);
        }


        public void CaptchaFailed()
        {
            if(captchaQueue.Count > 0)
                captchaQueue.Dequeue().Invoke(false);

            CaptchaMainContainer.SetActive(false);
            Pauser.Unpause(CaptchaMainContainer);
        }
    }
}
