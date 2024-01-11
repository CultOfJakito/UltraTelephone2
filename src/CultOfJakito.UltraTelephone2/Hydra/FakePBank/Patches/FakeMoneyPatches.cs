using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using TMPro;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2
{
    [HarmonyPatch]
    public static class FakeMoneyPatches
    {
        [HarmonyPatch(typeof(GameProgressSaver))]
        [HarmonyPatch(nameof(GameProgressSaver.GetMoney)), HarmonyPrefix]
        public static bool SwitcherooFakeMoney(ref int __result)
        {
            __result = (int) FakeBank.GetCurrentMoney();
            return false;
        }

        [HarmonyPatch(typeof(GameProgressSaver))]
        [HarmonyPatch(nameof(GameProgressSaver.AddMoney)), HarmonyPostfix]
        public static void SwitcherooFakeMoneyAdd(int money)
        {
            Debug.Log($"Adding fake money {money}");
            FakeBank.AddMoney(money);
        }

        [HarmonyPatch(typeof(MoneyText))]
        [HarmonyPatch(nameof(MoneyText.DivideMoney)), HarmonyPrefix]
        public static bool FixPString(ref string __result, int dosh)
        {
            if (dosh == int.MaxValue)
                __result = "∞";
            else if(dosh == int.MinValue)
                __result = "-∞";
            else
                __result = dosh.ToString("N0");

            return false;
        }

        [HarmonyPatch(typeof(MoneyText))]
        [HarmonyPatch(nameof(MoneyText.UpdateMoney)), HarmonyPostfix]
        public static void FixMoneyDisplay(MoneyText __instance)
        {
            TMP_Text text = __instance.GetComponent<TMP_Text>();
            text.text = FakeBank.FormatMoney(FakeBank.GetCurrentMoney()) + "<color=orange>P</color>";
        }
    }
}
