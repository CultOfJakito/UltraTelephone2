using CultOfJakito.UltraTelephone2.Data;
using TMPro;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Chaos.Effects.MoneyMania
{
    internal class CurrencyHUD : MonoBehaviour
    {
        private static CurrencyHUD s_instance;

        public static CurrencyHUD Instance
        {
            get
            {
                if (s_instance == null)
                {
                    s_instance = FindObjectOfType<CurrencyHUD>();
                }

                return s_instance;
            }
        }

        private void Awake()
        {
            s_instance = this;
        }

        public void Start()
        {
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
            RingsCounter.text = UT2SaveData.SaveData.Rings.ToString();
        }
        public void UpdateVbucksCounter()
        {
            VbucksCounter.text = UT2SaveData.SaveData.Vbucks.ToString();
        }
        public void UpdateBloodCounter()
        {
            BloodCounter.text = UT2SaveData.SaveData.Blood.ToString();
        }
        public void UpdateMetalScrapsCounter()
        {
            MetalScrapsCounter.text = UT2SaveData.SaveData.MetalScraps.ToString();
        }
        public void UpdateTrophiesCounter()
        {
            TrophiesCounter.text = UT2SaveData.SaveData.Trophies.ToString();
        }
        public void UpdateGunpowderCounter()
        {
            GunpowderCounter.text = UT2SaveData.SaveData.Gunpowder.ToString();
        }
        public void UpdateFishCounter()
        {
            FishCounter.text = UT2SaveData.SaveData.Fish.ToString();
        }
        public void UpdatePlushiesCounter()
        {
            PlushiesCounter.text = UT2SaveData.SaveData.Plushies.ToString();
        }
        #endregion

        public void UpdateMarketCoinCounter()
        {
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
