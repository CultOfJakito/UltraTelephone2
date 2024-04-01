using System;
using System.Collections.Generic;
using System.Text;
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
        }

        //CurrencyChaos
        [SerializeField] private TMP_Text RingsCounter;
        [SerializeField] private TMP_Text VbucksCounter; // TODO; add to main menu buying them
        [SerializeField] private TMP_Text BloodCounter;
        [SerializeField] private TMP_Text MetalScrapsCounter;
        [SerializeField] private TMP_Text TrophiesCounter;
        [SerializeField] private TMP_Text GunpowderCounter;
        [SerializeField] private TMP_Text FishCounter;
        [SerializeField] private TMP_Text PlushiesCounter;

        //yaya this can probably be some array and infinitely expandable but we have like no time finish this mod so im GO GO GOING!!
        //marketcoin counter
        [SerializeField] private TMP_Text Steelounter;
        [SerializeField] private TMP_Text BronzeCoinCounter;
        [SerializeField] private TMP_Text BrassCoinCounter;
        [SerializeField] private TMP_Text SilverCoinCounter;
        [SerializeField] private TMP_Text GoldCoinCounter;
        [SerializeField] private TMP_Text PlatinumCoinCounter;
        [SerializeField] private TMP_Text UltraiteCoinCounter;

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

        // im going to kill myself after this releases :pray:
        public void UpdateMarketCoinCounter()
        {
            int coint = UT2SaveData.SaveData.MarketCoins;

            int ultraCoint = (int)Math.Floor(coint / 10000000d);
            UltraiteCoinCounter.text = ultraCoint.ToString();
            coint -= ultraCoint * 10000000;

            int platCoint = (int)Math.Floor(coint / 1000000d);
            PlatinumCoinCounter.text = platCoint.ToString();
            coint -= platCoint * 1000000;

            int goldCoins = (int)Math.Floor(coint / 5000000d);
            GoldCoinCounter.text = goldCoins.ToString();
            coint -= goldCoins * 5000000;

            int silverCoint = (int)Math.Floor(coint / 100000d);
            SilverCoinCounter.text = silverCoint.ToString();
            coint -= silverCoint * 100000;

            int brassCoint = (int)Math.Floor(coint / 10000d);
            BrassCoinCounter.text = brassCoint.ToString();
            coint -= brassCoint * 10000;

            int bronzeCoint = (int)Math.Floor(coint / 1000d);
            BronzeCoinCounter.text = bronzeCoint.ToString();
            coint -= bronzeCoint * 1000;

            Steelounter.text = coint.ToString();
        }
    }

}
