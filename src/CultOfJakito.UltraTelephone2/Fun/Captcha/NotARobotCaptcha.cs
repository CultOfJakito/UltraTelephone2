using System;
using System.Collections.Generic;
using System.Text;
using CultOfJakito.UltraTelephone2.Chaos;
using CultOfJakito.UltraTelephone2.Chaos.Effects;
using CultOfJakito.UltraTelephone2.Fun.FakePBank;
using CultOfJakito.UltraTelephone2.Util;
using UnityEngine;
using UnityEngine.UI;

namespace CultOfJakito.UltraTelephone2.Fun.Captcha
{
    public class NotARobotCaptcha : Captcha
    {
        public Button toggleButton;
        public GameObject checkMark;
        public GameObject loadingCircle;

        public override void ShowCaptcha()
        {
            checkMark.SetActive(false);
            loadingCircle.SetActive(false);
            toggleButton.onClick.AddListener(Button_OnClick);
            toggleButton.interactable = true;
            gameObject.SetActive(true);
        }

        private void Button_OnClick()
        {
            toggleButton.interactable = false;
            loadingCircle.SetActive(true);

            UniRandom rand = new UniRandom(new SeedBuilder()
                .WithGlobalSeed()
                .WithObjectHash(Time.time));

            float delay = rand.Range(1f, 3f);

            if (rand.Chance(0.66f))
            {
                delay *= 0.3f;

                this.DoAfterTimeUnscaled(delay, () =>
                {
                    checkMark.SetActive(true);
                    loadingCircle.SetActive(false);
                    this.DoAfterTimeUnscaled(1f, () =>
                    {
                        m_manager.CaptchaDone();
                    });
                });

            }
            else
            {
                this.DoAfterTimeUnscaled(delay, () =>
                {
                    this.DoAfterTimeUnscaled(0.6f, () =>
                    {
                        if (rand.Chance(0.1f))
                        {
                            float finePercent = rand.Range(0.02f, 0.06f);
                            long fine = (long)(FakeBank.GetCurrentMoney() * finePercent);

                            string fineText = FakeBank.PString(fine);

                            AnnoyingPopUp.OkDialogue("You are a liar", "You have been fined for lying.", fineText, () =>
                            {
                                FakeBank.AddMoney(-fine);
                            });
                        }
                        else
                        {
                            m_manager.CaptchaFailed();
                        }
                    });
                });
            }
        }

        public override void HideCaptcha()
        {
            checkMark.SetActive(false);
            loadingCircle.SetActive(false);
            toggleButton.onClick.RemoveAllListeners();
            gameObject.SetActive(false);
        }
    }
}
