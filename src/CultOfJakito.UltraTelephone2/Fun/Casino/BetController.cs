using CultOfJakito.UltraTelephone2.UI;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Fun.Casino
{
    public class BetController : MonoBehaviour
    {
        public ClimbingText climbingText;

        public bool ValidateBetAmount = true;

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

            UpdateBetText();
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

            if(ValidateBetAmount)
            {
                long newBet = Math.Min(CasinoManager.Instance.Chips, Math.Max(0L, BetAmount + amount));
                BetAmount = newBet;
            }
            else
            {
                BetAmount = Math.Max(0L, BetAmount + amount);
            }

            
            UpdateBetText();
        }

        public void MultiplyBet(float multiplier)
        {
            if (locked)
                return;

            if (ValidateBetAmount)
            {
                long newBet = Math.Min(CasinoManager.Instance.Chips, Math.Max(0L, (long)(BetAmount * multiplier)));
                BetAmount = newBet;
            }
            else
            {
                BetAmount = Math.Max(0L, (long)(BetAmount * multiplier));
            }

            UpdateBetText();
        }


        public void UpdateBetText()
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

            if (ValidateBetAmount)
            {
                long newBet = Math.Min(CasinoManager.Instance.Chips, lastBetAmount);
                BetAmount = newBet;
            }
            else
            {
                BetAmount = lastBetAmount;
            }
            
            UpdateBetText();
        }
    }
}
