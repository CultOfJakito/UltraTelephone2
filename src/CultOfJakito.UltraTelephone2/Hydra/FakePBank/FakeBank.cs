using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2
{
    public static class FakeBank
    {
        private static bool initialized;

        public static long GetCurrentMoney()
        {
            Initialize();
            return UT2Data.SaveData.fakePAmount;
        }

        public static void AddMoney(long amount)
        {
            Initialize();
            UT2Data.SaveData.fakePAmount += amount;
            UT2Data.Save();
        }

        public static void SetMoney(long amount)
        {
            Initialize();
            UT2Data.SaveData.fakePAmount = amount;
            UT2Data.Save();
        }

        private static void Initialize()
        {
            if (initialized)
                return;

            //Dont use GetMoney() here, it will cause a stack overflow
            int p = GameProgressSaver.GetGeneralProgress().money;
            int lastP = UT2Data.SaveData.lastRealPAmount;
            UT2Data.SaveData.lastRealPAmount = p;

            if (!UT2Data.SaveData.initializedPAmount)
            {
                lastP = p;
                UT2Data.SaveData.fakePAmount = p;
                UT2Data.SaveData.initializedPAmount = true;
                UT2Data.Save();
            }

            if (p != lastP)
                UT2Data.SaveData.fakePAmount += p - lastP;


            initialized = true;
        }

        public static string FormatMoney(long money)
        {
            if(money == long.MaxValue)
                return "∞";
            else if(money == long.MinValue)
                return "-∞";

            return money.ToString("N0");
        }

        public static string PString(long money)
        {
            return "<color=white>" + FormatMoney(money) + "</color>" + "<color=orange>P</color>";
        }

    }
}
