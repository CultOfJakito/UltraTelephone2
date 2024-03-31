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
        public static void Tax(FinalRank __instance, ref int points)
        {
            if (!s_enabled.Value)
                return;

            float taxRate = (Utility.UserIsDeveloper()) ? 0.15f : 0.3f;

            float tax = points * taxRate;

            //Round up >:3c
            int taxAmount = Mathf.CeilToInt(tax);
            __instance.extraInfo.text += GetTaxInfoString(taxAmount);

            points = Mathf.Max(0, points - taxAmount);
        }

        private static string GetTaxInfoString(int taxes)
        {
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
            sb.Append(FakeBank.FormatMoney(taxes));
            sb.Append("</color>\n");

           return sb.ToString();

        }
    }
}
