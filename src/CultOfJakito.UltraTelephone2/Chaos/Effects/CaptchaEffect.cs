using System;
using System.Collections.Generic;
using System.Text;
using Configgy;
using CultOfJakito.UltraTelephone2.Data;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using CultOfJakito.UltraTelephone2.Events;
using CultOfJakito.UltraTelephone2.Fun.Captcha;
using HarmonyLib;

namespace CultOfJakito.UltraTelephone2.Chaos.Effects
{
    [HarmonyPatch]
    [RegisterChaosEffect]
    public class CaptchaEffect : ChaosEffect
    {
        [Configgable("Chaos/Effects", "Capthca On Death")]
        private static ConfigToggle s_enabled = new ConfigToggle(true);

        private static UniRandom s_rng;

        public override void BeginEffect(UniRandom random)
        {
            s_rng = random;
            GameEvents.OnPlayerRespawn += OnPlayerRespawn;
        }

        public override bool CanBeginEffect(ChaosSessionContext ctx) => s_enabled.Value && base.CanBeginEffect(ctx);

        public override int GetEffectCost()
        {
            return 2;
        }

        private void OnPlayerRespawn(PlayerRespawnEvent e)
        {
            if (e.IsManualRespawn)
                return;

            if (s_rng.Chance(0.33f))
                return;

            if (!s_enabled.Value)
                return;

            CaptchaManager.ShowCaptcha();
        }

        protected override void OnDestroy()
        {
            GameEvents.OnPlayerRespawn -= OnPlayerRespawn;
        }


    }
       
}
