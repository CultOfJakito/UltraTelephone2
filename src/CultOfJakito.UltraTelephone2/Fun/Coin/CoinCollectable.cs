using Configgy;
using CultOfJakito.UltraTelephone2.Assets;
using CultOfJakito.UltraTelephone2.Chaos;
using CultOfJakito.UltraTelephone2.Chaos.Effects.CurrencyChaos;
using CultOfJakito.UltraTelephone2.Events;
using CultOfJakito.UltraTelephone2.Fun.FakePBank;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Fun.Coin
{
    public class CoinCollectable : MonoBehaviour
    {
        [Configgable("Fun", "Enemies Drop Coins")]
        private static ConfigToggle s_enemiesDropCoinsOnDeath = new ConfigToggle(true);

        private MeshRenderer meshRenderer;
        private CoinType type;
        public long Value = 1;
        public Rigidbody rb;
        public bool specialCoin;

        private float lifeTime = 10f;
        public bool isCollectable = true;

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
            if (other.CompareTag("Player") && isCollectable)
            {
                Collected();
                return;
            }

            if (other.TryGetComponent(out HurtZone _))
                Dispose();

            if (other.TryGetComponent(out OutOfBounds _))
                Dispose();

            if (other.TryGetComponent(out DeathZone _))
                Dispose();
        }

        private bool destroyed;
        private void Dispose()
        {
            if (destroyed)
                return;

            destroyed = true;
            GameObject.Destroy(gameObject);
        }

        private void Collected()
        {
            FakeBank.AddMoney(Value);
            Instantiate(HydraAssets.CoinCollectFX, transform.position, transform.rotation);

            if (specialCoin)
                RandomCoinAtPoint(transform.position);

            if (type == CoinType.Ring)
                CurrencyChaos.InvokeRingCollected();

            Dispose();
        }

        private static PhysicMaterial mildlyBouncy;

        private static CoinCollectable NewCoin(GameObject coinObj, CoinType coinType)
        {
            coinObj.transform.localScale = Vector3.one * 0.6f;
            CapsuleCollider capsule = coinObj.GetComponent<CapsuleCollider>();
            capsule.radius *= 1.7f;

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

            coin.type = coinType;

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
                case CoinType.Ring:
                    return 0;
                default:
                    return 30;
            }
        }

        public static CoinType RollRarity(UniRandom rand)
        {
            if (CurrencyChaos.EffectActive)
            {
                if (rand.Chance(0.99f))
                {
                    return CoinType.Ring;
                }
            }

            if (rand.Chance(0.90f))
                return CoinType.Normal;

            if (rand.Chance(0.001f))
                return CoinType.Diamond;

            if (rand.Chance(0.05f))
                return CoinType.Black;

            if (rand.Chance(0.2f))
                return CoinType.Yellow;

            if (rand.Chance(0.35f))
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
                case CoinType.Ring:
                    return HydraAssets.CoinRing;
                default:
                    return HydraAssets.Coin;
            }
        }

        public static CoinCollectable CreateCoin(long value, CoinType type)
        {
            Console.WriteLine("Making a fucking coin!!");
            GameObject obj = Instantiate(GetCoinPrefab(type));
            CoinCollectable coin = NewCoin(obj, type);
            coin.Value = value;
            return coin;
        }

        public static CoinCollectable RandomCoinAtPoint(Vector3 point, UniRandom rand = null)
        {
            rand ??= UniRandom.CreateFullRandom();

            CoinType type = RollRarity(rand);
            long value = GetValue(type);
            CoinCollectable coin = CreateCoin(value, type);

            coin.specialCoin = type == CoinType.Normal && rand.Chance(0.05f);
            coin.transform.position = point + new Vector3(rand.Range(-0.2f, 0.2f), 0, rand.Range(-0.2f, 0.2f));

            Vector3 force = rand.UnitSphere();
            force.y = 2.34f;
            force = force.normalized * 17f;
            coin.rb.velocity = force;

            return coin;
        }

        public static CoinCollectable ThrowRandomCoin(Vector3 point, Vector3 velocity, UniRandom rand = null)
        {
            rand ??= UniRandom.CreateFullRandom();

            CoinType type = RollRarity(rand);
            long value = GetValue(type);
            CoinCollectable coin = CreateCoin(value, type);

            coin.specialCoin = type == CoinType.Normal && rand.Chance(0.05f);
            coin.transform.position = point + new Vector3(rand.Range(-0.2f, 0.2f), 0, rand.Range(-0.2f, 0.2f));
            coin.rb.velocity = velocity;

            return coin;
        }

        public static void OnEnemyDeath(EnemyDeathEvent enemyDeath)
        {
            if (!s_enemiesDropCoinsOnDeath.Value)
                return;

            //Don't count as kills so no coins are dropped
            if (enemyDeath.Enemy.dontCountAsKills)
                return;

            UniRandom rand = UniRandom.CreateFullRandom();

            if (rand.Chance(0.01f))
            {
                enemyDeath.Enemy.gameObject.AddComponent<CoinFountain>();
                return;
            }

            Vector3 point = enemyDeath.Enemy.transform.position;
            point.y += 0.75f;

            int coinsToDrop = rand.Range(2, 10);

            for (int i = 0; i < coinsToDrop; i++)
            {
                RandomCoinAtPoint(point, rand);
            }
        }
    }

    public enum CoinType
    {
        Ring,
        Normal,
        Blue,
        Red,
        Yellow,
        Black,
        Diamond
    }
}
