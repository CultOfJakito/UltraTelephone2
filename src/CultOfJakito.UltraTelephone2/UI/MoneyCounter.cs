using System;
using System.Collections.Generic;
using System.Text;
using CultOfJakito.UltraTelephone2.Hydra.FakePBank;
using TMPro;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.UI
{
    public class MoneyCounter : MonoBehaviour
    {
        public ClimbingText climbingText;


        private void Start()
        {
            long money = FakeBank.GetCurrentMoney();
            climbingText.toString = FakeBank.PString;
            climbingText.SetValue(money);
            FakeBank.OnMoneyChanged += FakeBank_OnMoneyChanged;
        }

        private void FakeBank_OnMoneyChanged(long obj)
        {
            climbingText.SetTargetValue(obj);
        }

        private void OnDestroy()
        {
            FakeBank.OnMoneyChanged -= FakeBank_OnMoneyChanged;
        }
    }
}
