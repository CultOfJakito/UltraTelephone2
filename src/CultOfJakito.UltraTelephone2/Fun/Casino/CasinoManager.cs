using System;
using System.Collections.Generic;
using System.Text;
using CultOfJakito.UltraTelephone2.Fun.FakePBank;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Fun.Casino
{
    [ConfigureSingleton(SingletonFlags.NoAutoInstance)]
    public class CasinoManager : MonoSingleton<CasinoManager>
    {
        public long Winnings;
        public long Losings;
        public long Chips { get; private set; }

        public long ChipsBought;
        private bool ambushActive;

        public UltrakillEvent onAmbushStart;
        public event Action<long> OnChipsChanged;

        public void BuyChips(long amount)
        {
            long money = FakeBank.GetCurrentMoney();
            long addition = Math.Min(money, amount);

            Chips += addition;
            ChipsBought += addition;

            if(addition != 0)
                FakeBank.AddMoney(-addition);

            OnChipsChanged?.Invoke(Chips);
        }

        public void SellChips(long amount)
        {
            if (amount <= 0)
                return;

            long subtraction = Math.Min(Chips, amount);
            Chips -= subtraction;

            if(subtraction != 0)
                FakeBank.AddMoney(subtraction);
        }

        public void AddChips(long amount)
        {
            Chips += amount;
            OnChipsChanged?.Invoke(Chips);
        }

        public void SetChips (long amount)
        {
            Chips = amount;
            OnChipsChanged?.Invoke(Chips);
        }

        public bool CanAmbush()
        {
            long profit = Chips - ChipsBought;
            float multiplier = (float)((double)profit/(double)ChipsBought);
            return multiplier > 500f && Chips > 1000000;
        }

        public void Ambush()
        {
            if (!CanAmbush())
                return;

            ambushActive = true;
            onAmbushStart?.Invoke();

            //Just start endlessly spamming gutterman and guttertank until player dies or like 100 kills ig
            //THEY GOTTA EARN IT.
        }

        public void Cashout()
        {
            if (Chips <= 0 || ambushActive)
                return;

            long profit = Chips - ChipsBought;

            FakeBank.AddMoney(Chips);
            Chips = 0;
            ChipsBought -= profit;
        }

        public static string FormatChips(long chips)
        {
            return $"{FakeBank.FormatMoney(chips)} <color=blue>C</color>";
        }
    }
}
