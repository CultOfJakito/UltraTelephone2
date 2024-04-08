using CultOfJakito.UltraTelephone2.Data;
using TMPro;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Chaos.Effects.MoneyMania
{
    internal class CurrencyHUD : MonoBehaviour
    {
        public static CurrencyHUD Instance;

        public void Start()
        {
            Instance = this;
            UpdateAllCounters();
        }

        //CurrencyChaos
        [SerializeField] private TMP_Text RingsCounter;
        [SerializeField] private TMP_Text VbucksCounter;
        [SerializeField] private TMP_Text BloodCounter;
        [SerializeField] private TMP_Text MetalScrapsCounter;
        [SerializeField] private TMP_Text TrophiesCounter;
        [SerializeField] private TMP_Text GunpowderCounter;
        [SerializeField] private TMP_Text FishCounter;
        [SerializeField] private TMP_Text PlushiesCounter;

        [SerializeField] private TMP_Text[] CoinCountersTexts;
        [SerializeField] private double[] CoinCountersWorths;

        public void UpdateAllCounters()
        {
            UpdateRingsCounter();
            UpdateVbucksCounter();
            UpdateBloodCounter();
            UpdateMetalScrapsCounter();
            UpdateTrophiesCounter();
            UpdateGunpowderCounter();
            UpdateFishCounter();
            UpdatePlushiesCounter();
            UpdateMarketCoinCounter();
        }
        #region Update Various Counters
        public void UpdateRingsCounter()
        {
            Console.WriteLine("Updating Ring Counter!");
            RingsCounter.text = UT2SaveData.SaveData.Rings.ToString();
        }
        public void UpdateVbucksCounter()
        {
            Console.WriteLine("Updating Vbucks!");
            VbucksCounter.text = UT2SaveData.SaveData.Vbucks.ToString();
        }
        public void UpdateBloodCounter()
        {
            Console.WriteLine("Updating Blood Counter!");
            BloodCounter.text = UT2SaveData.SaveData.Blood.ToString();
        }
        public void UpdateMetalScrapsCounter()
        {
            Console.WriteLine("Updating Scrap Counter!");
            MetalScrapsCounter.text = UT2SaveData.SaveData.MetalScraps.ToString();
        }
        public void UpdateTrophiesCounter()
        {
            Console.WriteLine("Updating Trophies Counter!");
            TrophiesCounter.text = UT2SaveData.SaveData.Trophies.ToString();
        }
        public void UpdateGunpowderCounter()
        {
            Console.WriteLine("Updating Gunpowder Counter!");
            GunpowderCounter.text = UT2SaveData.SaveData.Gunpowder.ToString();
        }
        public void UpdateFishCounter()
        {
            Console.WriteLine("Updating Fish Counter!");
            FishCounter.text = UT2SaveData.SaveData.Fish.ToString();
        }
        public void UpdatePlushiesCounter()
        {
            Console.WriteLine("Updating Plushies Counter!");
            PlushiesCounter.text = UT2SaveData.SaveData.Plushies.ToString();
        }
        #endregion

        public void UpdateMarketCoinCounter()
        {
            Console.WriteLine("Updating Coin Counter!");
            int coint = UT2SaveData.SaveData.MarketCoins;


            // make sure to have the elements in order of worth most to least!
            int i = 0;
            foreach(TMP_Text text in CoinCountersTexts)
            {
                double value = CoinCountersWorths[i];
                int coinValue = (int)Math.Floor(coint / value);
                text.text = coinValue.ToString();
                coint -= coinValue * (int)value;
                i++;
            }
        }
    }

}
