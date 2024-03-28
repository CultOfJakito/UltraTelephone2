using System;
using System.Collections.Generic;
using System.Text;
using Configgy;
using CultOfJakito.UltraTelephone2.Chaos;
using CultOfJakito.UltraTelephone2.Data;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using HarmonyLib;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Hydra.Chaos
{
    [HarmonyPatch]
    [RegisterChaosEffect]
    public class CantRead : ChaosEffect
    {
        [Configgable("Chaos/Effects", "Cant Read")]
        private static ConfigToggle s_enabled = new ConfigToggle(true);

        private static bool s_effectActive = false;

        private static List<string> s_illegebleWords => UT2TextFiles.S_CantReadWordsFile.TextList;

        private static UniRandom s_rng;

        public override void BeginEffect(UniRandom random)
        {
            s_rng = random;
            s_effectActive = true;
        }

        public override bool CanBeginEffect(ChaosSessionContext ctx) => s_enabled.Value && base.CanBeginEffect(ctx);

        public override int GetEffectCost()
        {
            return 2;
        }

        public override void Dispose()
        {
            s_effectActive = false;
            base.Dispose();
        }

        [HarmonyPatch(typeof(ScanningStuff), nameof(ScanningStuff.ScanBook)), HarmonyPrefix]
        private static void OnScanBook(ScanningStuff __instance, ref string text)
        {
            if (!s_enabled.Value || !s_effectActive)
                return;

            int words = text.Split(' ').Length;

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < words; i++)
            {
                sb.Append(s_rng.SelectRandomList(s_illegebleWords));
            }

            text = sb.ToString();
        }
    }
}
