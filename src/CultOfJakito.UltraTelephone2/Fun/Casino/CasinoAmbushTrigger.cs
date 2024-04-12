using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Fun.Casino
{
    public class CasinoAmbushTrigger : MonoBehaviour
    {

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player"))
                return;

            CasinoManager.Instance.Ambush();

        }
    }
}
