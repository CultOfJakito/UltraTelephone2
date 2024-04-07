using System;
using System.Collections.Generic;
using System.Text;
using CultOfJakito.UltraTelephone2.Data;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Chaos.Effects.MoneyMania
{
    internal class VBucksPurchaseUI : MonoBehaviour
    {


        [SerializeField] private int VBucksAmount; 
        private readonly string _charityURL = "https://outrightinternational.org/take-action/give/";

        public void GoToCharityWebsite()
        {
            UT2SaveData.SaveData.Vbucks += VBucksAmount;
            UT2SaveData.MarkDirty();
            Application.OpenURL(_charityURL);
        }
    }
}
