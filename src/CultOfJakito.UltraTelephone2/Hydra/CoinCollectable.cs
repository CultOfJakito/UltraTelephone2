﻿using System;
using System.Collections.Generic;
using System.Text;
using Configgy;
using CultOfJakito.UltraTelephone2.Assets;
using CultOfJakito.UltraTelephone2.Events;
using CultOfJakito.UltraTelephone2.Hydra.FakePBank;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Hydra
{
    public class CoinCollectable : MonoBehaviour
    {
        [Configgable("Fun", "Enemies Drop Coins")]
        private static ConfigToggle s_enemiesDropCoinsOnDeath = new ConfigToggle(true);

        private MeshRenderer meshRenderer;
        public long Value = 1;
        public Rigidbody rb;
        public bool specialCoin;

        private float lifeTime = 10f;

        private void Start()
        {
            meshRenderer = GetComponentInChildren<MeshRenderer>();
            gameObject.AddComponent<FloatingPointErrorPreventer>();
        }

        private void Update()
        {
            lifeTime = Mathf.Max(0, lifeTime - Time.deltaTime);

            bool blink = lifeTime < 4f && (int)(lifeTime * 10) % 2 == 0;
            meshRenderer.enabled = !blink;

            if (lifeTime <= 0)
                Destroy(gameObject);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player"))
                return;

            Collected();
        }

        private void Collected()
        {
            FakeBank.AddMoney(Value);
            GameObject.Instantiate(HydraAssets.CoinCollectFX, transform.position, transform.rotation);

            if (specialCoin)
                RandomCoinAtPoint(transform.position);

            Destroy(gameObject);
        }

        private static PhysicMaterial mildlyBouncy;

        private static CoinCollectable NewCoin(GameObject coinObj)
        {
            coinObj.transform.localScale = Vector3.one * 0.6f;
            CoinCollectable coin = coinObj.AddComponent<CoinCollectable>();
            coin.rb = coin.gameObject.AddComponent<Rigidbody>();
            coin.rb.constraints = RigidbodyConstraints.FreezeRotation;
            coin.rb.mass = 0.2f;

            SphereCollider rigidCollider = coinObj.AddComponent<SphereCollider>();
            rigidCollider.radius = 0.35f;
            rigidCollider.center = new Vector3(0, 0.35f, 0);

            mildlyBouncy ??= new PhysicMaterial
            {
                dynamicFriction = 0.8f,
                staticFriction = 0.8f,
                bounciness = 0.4f,
                frictionCombine = PhysicMaterialCombine.Average,
                bounceCombine = PhysicMaterialCombine.Maximum
            };

            rigidCollider.material = mildlyBouncy;

            return coin;
        }

        public static long GetValue(CoinType type)
        {
            switch (type)
            {
                case CoinType.Blue:
                    return 300;
                case CoinType.Red:
                    return 1000;
                case CoinType.Yellow:
                    return 10000;
                case CoinType.Black:
                    return 100000;
                case CoinType.Diamond:
                    return 10000000;
                default:
                    return 30;
            }
        }

        public static CoinType RollRarity(UniRandom rand)
        {
            if (rand.Chance(0.90f))
                return CoinType.Normal;

            if(rand.Chance(0.0001f))
                return CoinType.Diamond;

            if(rand.Chance(0.01f))
                return CoinType.Black;

            if(rand.Chance(0.1f))
                return CoinType.Yellow;

            if(rand.Chance(0.3f))
                return CoinType.Red;

            return CoinType.Blue;
        }

        private static GameObject GetCoinPrefab(CoinType type)
        {
            switch (type)
            {
                case CoinType.Blue:
                    return HydraAssets.CoinBlue;
                case CoinType.Red:
                    return HydraAssets.CoinRed;
                case CoinType.Yellow:
                    return HydraAssets.CoinYellow;
                case CoinType.Black:
                    return HydraAssets.CoinBlack;
                case CoinType.Diamond:
                    return HydraAssets.CoinDiamond;
                default:
                    return HydraAssets.Coin;
            }
        }

        public static CoinCollectable CreateCoin(long value, CoinType type)
        {
            GameObject obj = GameObject.Instantiate(GetCoinPrefab(type));
            CoinCollectable coin = NewCoin(obj);
            coin.Value = value;
            return coin;
        }

        public static void RandomCoinAtPoint(Vector3 point, UniRandom rand = null)
        {
            rand ??= UniRandom.CreateFullRandom();

            CoinType type = RollRarity(rand);
            long value = GetValue(type);
            CoinCollectable coin = CreateCoin(value, type);

            coin.specialCoin = type == CoinType.Normal && rand.Chance(0.05f);
            coin.transform.position = point + new Vector3(rand.Range(-0.2f, 0.2f), 0, rand.Range(-0.2f, 0.2f));

            Vector3 force = rand.UnitSphere();
            force.y = 2.34f;
            force = force.normalized * 15f;
            coin.rb.velocity = force;
        }

        public static void OnEnemyDeath(EnemyDeathEvent enemyDeath)
        {
            if (!s_enemiesDropCoinsOnDeath.Value)
                return;
            UniRandom rand = UniRandom.CreateFullRandom();

            if (rand.Chance(0.01f))
            {
                enemyDeath.Enemy.gameObject.AddComponent<CoinFountain>();
                return;
            }

            Vector3 point = enemyDeath.Enemy.transform.position;
            point.y += 0.75f;

            int coinsToDrop = rand.Range(1, 6);

            for (int i = 0; i < coinsToDrop; i++)
            {
                RandomCoinAtPoint(point, rand);
            }
        }
    }

    public enum CoinType
    {
        Normal,
        Blue,
        Red,
        Yellow,
        Black,
        Diamond
    }
}
