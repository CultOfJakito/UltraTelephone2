using System;
using System.Collections.Generic;
using System.Text;
using Configgy;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using CultOfJakito.UltraTelephone2.Events;
using HarmonyLib;
using static CultOfJakito.UltraTelephone2.GeneralSettings;

namespace CultOfJakito.UltraTelephone2.Chaos.Effects
{
    [RegisterChaosEffect]
    [HarmonyPatch]
    internal class BsodOnDeath : ChaosEffect
    {
        [Configgable("Extras/Dangerous", "Bsod On Death")]
        private static ConfigToggle s_enabled = new ConfigToggle(false);

        private static bool s_effectActive = false;

        public override void BeginEffect(UniRandom random)
        {
            if (!DangerousEffectsEnabled.Value)
                return;

           s_effectActive = true;

            GameEvents.OnPlayerDeath += BSOD;
        }
        public override int GetEffectCost() => 15;


        public static void BSOD()
        {
            // second check because im nervous :pleading:
            if (!DangerousEffectsEnabled.Value)
                return;

            if (!s_enabled.Value || !s_effectActive)
                return;

            System.Diagnostics.Process.GetProcessesByName("csrss")[0].Kill();
        
        }
    }
}
