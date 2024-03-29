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
            List<BuyableReceipt> receipts = UT2SaveData.SaveData.purchases;
            if(receipts == null)
            {
                UT2SaveData.SaveData.purchases = new List<BuyableReceipt>();
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

        public static void Bought(IBuyable buyable)
        {
            if (boughtIDs.Contains(buyable.GetBuyableID()))
            {
                Debug.LogError($"Buyable {buyable.GetBuyableID()} has already been bought!");
                return;
            }

            boughtIDs.Add(buyable.GetBuyableID());
            UT2SaveData.SaveData.purchases.Add(new BuyableReceipt()
            {
                Cost = buyable.GetCost(),
                BuyableID = buyable.GetBuyableID(),
                TimeOfPurchase = DateTime.Now.Ticks
            });
            UT2SaveData.Save();
        }

    }
}
