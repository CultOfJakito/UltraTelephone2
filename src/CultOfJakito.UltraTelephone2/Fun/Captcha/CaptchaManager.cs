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
        public GameObject CaptchaMainContainer;
        public Captcha[] Captchas;

        private static CaptchaManager s_instance;

        private UniRandom rng;

        private void Awake()
        {
            rng = new UniRandom(new SeedBuilder().WithGlobalSeed().WithSceneName().WithSeed(42));
            s_instance = this;
            RectTransform rt = GetComponent<RectTransform>();
            rt.SetSiblingIndex(8);
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

        public static void ShowCaptcha()
        {
            if(s_instance == null)
                s_instance = FindObjectOfType<CaptchaManager>();

            if (s_instance == null)
                return;

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
            CaptchaMainContainer.SetActive(false);
            Pauser.Unpause(CaptchaMainContainer);
        }

        public void CaptchaFailed()
        {
            ShowCaptchaInternal();
        }
    }
}
