using System;
using System.Collections.Generic;
using System.Text;
using CultOfJakito.UltraTelephone2.Data;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Hydra.EA
{
    public static class BuyablesManager
    {
        private static Dictionary<string, BuyableReceipt> receiptLog = new Dictionary<string, BuyableReceipt>();

        private static HashSet<string> boughtIDs = new HashSet<string>();

        public static void Load()
        {
            List<BuyableReceipt> receipts = UT2Data.SaveData.purchases;
            if(receipts == null)
            {
                UT2Data.SaveData.purchases = new List<BuyableReceipt>();
                return;
            }

            foreach (BuyableReceipt receipt in receipts)
            {
                boughtIDs.Add(receipt.BuyableID);
            }
        }

        public static bool IsBought(string buyableID)
        {
            return receiptLog.ContainsKey(buyableID);
        }

        public static void Bought(string buyableID)
        {
            if (boughtIDs.Contains(buyableID))
            {
                Debug.LogError($"Buyable {buyableID} has already been bought!");
                return;
            }

            boughtIDs.Add(buyableID);
            UT2Data.SaveData.purchases.Add(new BuyableReceipt()
            {
                BuyableID = buyableID,
                TimeOfPurchase = DateTime.Now.Ticks
            });
            UT2Data.Save();
        }

    }
}
