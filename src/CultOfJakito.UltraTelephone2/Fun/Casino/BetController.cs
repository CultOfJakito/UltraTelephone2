using System;
using System.Collections.Generic;
using System.Text;
using CultOfJakito.UltraTelephone2.Fun.FakePBank;
using CultOfJakito.UltraTelephone2.UI;
using TMPro;
using UnityEngine;
using static Steamworks.InventoryItem;

namespace CultOfJakito.UltraTelephone2.Fun.Casino
{
    public class BetController : MonoBehaviour
    {
        public ClimbingText climbingText;

        public long BetAmount { get; private set; }
        private long lastBetAmount;
        private bool locked;

        private void Start()
        {
            climbingText.SetTargetValue(BetAmount);
            climbingText.toString = (v) =>
            {
                if (locked)
                {
                    return $"Bet: [{CasinoManager.FormatChips(BetAmount)}]";
                }
                else
                {
                    return $"Bet: {CasinoManager.FormatChips(BetAmount)}";
                }
            };
        }

        public void SetLocked(bool locked)
        {
            this.locked = locked;
            UpdateBetText();
        }

        public void AddBet(int amount)
        {
            if (locked)
                return;

            long newBet = Math.Min(CasinoManager.Instance.Chips, Math.Max(0L, BetAmount + amount));
            BetAmount = newBet;
            UpdateBetText();
        }

        public void MultiplyBet(float multiplier)
        {
            if (locked)
                return;

            long newBet = Math.Min(CasinoManager.Instance.Chips, Math.Max(0L, (long)(BetAmount * multiplier)));
            BetAmount = newBet;
            UpdateBetText();
        }


        private void UpdateBetText()
        {
            climbingText.SetTargetValue(BetAmount);
            climbingText.UpdateText();
        }

        public void ResetBet()
        {
            lastBetAmount = BetAmount;
            BetAmount = 0;
            UpdateBetText();
        }

        public void ClearBet()
        {
            if (locked)
                return;

            lastBetAmount = BetAmount;
            BetAmount = 0;
            UpdateBetText();
        }

        public void RestoreLastBet()
        {
            if (locked)
                return;

            long newBet = Math.Min(CasinoManager.Instance.Chips, lastBetAmount);
            BetAmount = newBet;
            UpdateBetText();
        }
    }
}
