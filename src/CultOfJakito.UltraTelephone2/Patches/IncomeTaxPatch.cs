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
    [HarmonyPatch]
    public static class IncomeTaxPatch
    {
        [Configgable("Patches", "Taxes")]
        private static ConfigToggle s_enabled = new(true);


        [HarmonyPatch(typeof(FinalRank), nameof(FinalRank.AddPoints)), HarmonyPrefix]
        public static void Tax(ref int points)
        {
            if (!s_enabled.Value)
                return;

            float taxRate = (Utility.UserIsDeveloper()) ? 0.15f : 0.3f;

            float tax = points * taxRate;

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

            string rate = Utility.UserIsDeveloper() ? "15%" : "30%";
            StringBuilder sb = new();
            sb.Append("<color=red>Income Tax [");
            sb.Append(rate);
            sb.Append("] ");

            if (Utility.UserIsDeveloper())
            {
                sb.Append("<color=yellow>(DEV RATE)</color> ");
            }

            sb.Append("- ");
            sb.Append(FakeBank.FormatMoney(s_taxAmountBuffer));
            sb.Append("</color>\n");

            __instance.pointsText.text += sb.ToString();
        }
    }
}
