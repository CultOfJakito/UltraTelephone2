using System.Text;
using Configgy;
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
        public static void TaxDeduct(FinalRank __instance, ref int points)
        {
            if (!s_enabled.Value)
                return;

            float taxRate = (Utility.UserIsDeveloper()) ? 0.15f : 0.3f;

            float tax = points * taxRate;

            //Round up >:3c
            int taxAmount = Mathf.CeilToInt(tax);
            s_taxesTakenBuffer += taxAmount;
            points = Mathf.Max(0, points - taxAmount);
        }

        static int s_taxesTakenBuffer = 0;

        [HarmonyPatch(typeof(FinalRank), nameof(FinalRank.Appear)), HarmonyPostfix]
        public static void TaxAppear(FinalRank __instance)
        {
            if (!s_enabled.Value)
                return;

            if (s_taxesTakenBuffer <= 0)
                return;

            int taxTaken = s_taxesTakenBuffer;
            s_taxesTakenBuffer = 0;

            __instance.extraInfo.text += GetTaxInfoString(taxTaken);
        }

        private static string GetTaxInfoString(int taxes)
        {
            string rate = Utility.UserIsDeveloper() ? "15%" : "30%";
            StringBuilder sb = new();
            sb.Append("<color=red>- ");
            sb.Append(FakeBank.FormatMoney(taxes));
            sb.Append(" TAXES [");
            sb.Append(rate);
            sb.Append("]</color>\n");
            return sb.ToString();
        }
    }
}
