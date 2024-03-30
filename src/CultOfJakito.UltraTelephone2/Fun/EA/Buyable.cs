using CultOfJakito.UltraTelephone2.Fun.FakePBank;
using UnityEngine;
using UnityEngine.Events;

namespace CultOfJakito.UltraTelephone2.Fun.EA
{
    public class Buyable : MonoBehaviour, IBuyable
    {
        public UnityEvent OnBuy;
        public string BuyableID;
        public long Cost;

        public void Buy()
        {
            if (BuyablesManager.IsBought(BuyableID))
                return;

            long money = FakeBank.GetCurrentMoney();
            if (money < GetCost())
                return;

            long newMoney = money - GetCost();
            BuyablesManager.Bought(this);
            FakeBank.SetMoney(newMoney);
            OnBuy?.Invoke();
        }

        public string GetBuyableID() => BuyableID;
        public long GetCost() => Cost;
    }
}
