using CultOfJakito.UltraTelephone2.Util;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Boss
{
    public class TelephoneBossArena : MonoBehaviour
    {
        public GameObject[] Bosses;

        private bool _active = false;

        public void Activate()
        {
            if (_active)
                return;

            _active = true;
            UniRandom rand = new UniRandom(new SeedBuilder().WithGlobalSeed().GetSeed());
            rand.SelectRandom(Bosses).SetActive(true);
        }


    }
}
