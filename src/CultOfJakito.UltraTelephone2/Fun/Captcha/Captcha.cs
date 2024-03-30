using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Fun.Captcha
{
    public abstract class Captcha : MonoBehaviour
    {
        protected CaptchaManager m_manager;
        public void SetManager(CaptchaManager manager) => m_manager = manager;

        public abstract void ShowCaptcha();
        public abstract void HideCaptcha();
    }
}
