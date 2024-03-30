using System;
using System.Collections.Generic;
using System.Text;
using Configgy;
using CultOfJakito.UltraTelephone2.Chaos;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Hydra.Chaos
{
    [RegisterChaosEffect]
    public class RetroFPS : ChaosEffect
    {
        [Configgable("Chaos/Effects", "Retro FPS")]
        private static ConfigToggle s_enabled = new ConfigToggle(true);

        private static int? s_targetFps;

        public override void BeginEffect(UniRandom random)
        {
            if (s_targetFps == null)
                s_targetFps = Application.targetFrameRate;

            Application.targetFrameRate = 30;
        }

        public override void Dispose()
        {
            if (s_targetFps != null)
                Application.targetFrameRate = s_targetFps.Value;

            base.Dispose();
        }

        public override bool CanBeginEffect(ChaosSessionContext ctx) => s_enabled.Value && base.CanBeginEffect(ctx);

        public override int GetEffectCost() => 10;
    }
}
