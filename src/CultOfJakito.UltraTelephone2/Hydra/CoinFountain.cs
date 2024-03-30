using System;
using System.Collections.Generic;
using System.Text;
using CultOfJakito.UltraTelephone2.Util;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Hydra
{
    public class CoinFountain : MonoBehaviour
    {
        private float timeBetweenCoins = 0.05f;
        public int coinCount;
        private UniRandom random;

        private void Awake()
        {
            random = new UniRandom(new SeedBuilder().WithGlobalSeed().WithSceneName().WithObjectHash(gameObject));
            coinCount = random.Range(1, 10) * 100;
        }

        private void Start()
        {
            SpawnCoin();
        }

        private void SpawnCoin()
        {
            if (coinCount <= 0)
            {
                Destroy(this);
                return;
            }

            CoinCollectable.RandomCoinAtPoint(transform.position, random);
            --coinCount;

            Invoke(nameof(SpawnCoin), timeBetweenCoins);
        }
    }
}
