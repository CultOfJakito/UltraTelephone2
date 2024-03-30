using System;
using System.Collections.Generic;
using System.Text;
using Configgy;
using CultOfJakito.UltraTelephone2.Chaos;
using CultOfJakito.UltraTelephone2.Fun.FakePBank;
using HarmonyLib;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Patches
{
    public static class IncomeTaxPatch
    {
        [Configgable("Patch", "Taxes")]
        private static ConfigToggle s_enabled = new(true);


        [HarmonyPatch(typeof(FinalRank), nameof(FinalRank.AddPoints)), HarmonyPrefix]
        public static void Tax(ref int points)
        {
            if (!s_enabled.Value)
                return;

            float tax = points * 0.3f;

            //Round up >:3c
            int taxAmount = Mathf.CeilToInt(tax);

            s_taxAmountBuffer = taxAmount;
            points = Mathf.Max(0, points - taxAmount);
        }

        private static int s_taxAmountBuffer;

        [HarmonyPatch(typeof(FinalRank), nameof(FinalRank.PointsShow)), HarmonyPostfix]
        public static void TaxDisplay(FinalRank __instance)
        {
            if (!s_enabled.Value)
                return;

            if (s_taxAmountBuffer <= 0)
                return;

            __instance.pointsText.text += $"<color=red>Income Tax [30%] - {FakeBank.FormatMoney(s_taxAmountBuffer)}</color>";
        }
    }
}
