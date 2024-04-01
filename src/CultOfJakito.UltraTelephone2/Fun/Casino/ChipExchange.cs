using CultOfJakito.UltraTelephone2.Fun.FakePBank;
using CultOfJakito.UltraTelephone2.UI;
using TMPro;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Fun.Casino
{
    public class ChipExchange : MonoBehaviour
    {
        public TMP_Text chipBalanceText;
        public TMP_Text pBalanceText;
        public ClimbingText climbingText;
        public bool BuyMode;
        public bool ValidateChipBuffer = true;

        public void Process()
        {
            if (ChipBuffer <= 0)
                return;

            long chips = ChipBuffer;

            if (BuyMode)
                CasinoManager.Instance.BuyChips(chips);
            else
                CasinoManager.Instance.SellChips(chips);

            ResetBet();

        }


        public long ChipBuffer { get; private set; }
        private long lastChipBuffer;
        private bool locked;

        private void Start()
        {
            climbingText.SetTargetValue(ChipBuffer);
            climbingText.toString = FakeBank.FormatMoney;
            UpdateTexts();
        }

        public void SetLocked(bool locked)
        {
            this.locked = locked;
            UpdateTexts();
        }

        public void AddBet(int amount)
        {
            if (locked)
                return;

            if (ValidateChipBuffer)
            {
                long newBet = Math.Min(GetCurrentMoney(), Math.Max(0L, ChipBuffer + amount));
                ChipBuffer = newBet;
            }
            else
            {
                ChipBuffer = Math.Max(0L, ChipBuffer + amount);
            }


            UpdateTexts();
        }

        private long GetCurrentMoney()
        {
            if (BuyMode)
            {
                return FakeBank.GetCurrentMoney();
            }
            else
            {
                return CasinoManager.Instance.Chips;
            }
        }

        public void MultiplyBet(float multiplier)
        {
            if (locked)
                return;

            if (ValidateChipBuffer)
            {
                long newBet = Math.Min(GetCurrentMoney(), Math.Max(0L, (long)(ChipBuffer * multiplier)));
                ChipBuffer = newBet;
            }
            else
            {
                ChipBuffer = Math.Max(0L, (long)(ChipBuffer * multiplier));
            }

            UpdateTexts();
        }


        public void UpdateTexts()
        {
            climbingText.SetTargetValue(ChipBuffer);
            climbingText.UpdateText();

            chipBalanceText.text = CasinoManager.FormatChips(CasinoManager.Instance.Chips);
            pBalanceText.text = FakeBank.PString(FakeBank.GetCurrentMoney());
        }

        public void ResetBet()
        {
            lastChipBuffer = ChipBuffer;
            ChipBuffer = 0;
            UpdateTexts();
        }

        public void ClearBet()
        {
            if (locked)
                return;

            lastChipBuffer = ChipBuffer;
            ChipBuffer = 0;
            UpdateTexts();
        }

        public void RestoreLastBet()
        {
            if (locked)
                return;

            if (ValidateChipBuffer)
            {
                long newBet = Math.Min(GetCurrentMoney(), lastChipBuffer);
                ChipBuffer = newBet;
            }
            else
            {
                ChipBuffer = lastChipBuffer;
            }

            UpdateTexts();
        }
    }
}
