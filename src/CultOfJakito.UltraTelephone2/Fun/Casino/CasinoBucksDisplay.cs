using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Fun.Casino
{
    public class CasinoBucksDisplay : MonoBehaviour
    {
        private TMP_Text _text;

        private void OnEnable()
        {
            UpdateChips(CasinoManager.Instance.Chips);
            CasinoManager.Instance.OnChipsChanged += UpdateChips;
        }

        private void UpdateChips(long chips)
        {
            _text = GetComponent<TMP_Text>();
            _text.text = CasinoManager.FormatChips(CasinoManager.Instance.Chips);
        }

        private void OnDisable()
        {
            CasinoManager.Instance.OnChipsChanged -= UpdateChips;
        }
    }
}
