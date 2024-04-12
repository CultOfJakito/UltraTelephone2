using Configgy;
using CultOfJakito.UltraTelephone2.Assets;
using CultOfJakito.UltraTelephone2.Chaos.Effects.MoneyMania;
using CultOfJakito.UltraTelephone2.Data;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using CultOfJakito.UltraTelephone2.Events;
using HarmonyLib;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Chaos.Effects.CurrencyChaos
{
    [HarmonyPatch]
    [RegisterChaosEffect]
    internal class CurrencyChaos : ChaosEffect
    {
        [Configgable("Chaos/Effects", "Currency Chaos")]
        private static ConfigToggle s_enabled = new ConfigToggle(true);
        public static bool EffectActive { get; private set; }

        private static UniRandom s_random;

        private GameObject _currencyUI;

        private static event Action s_OnRingCollected;

        public override bool CanBeginEffect(ChaosSessionContext ctx) => s_enabled.Value && base.CanBeginEffect(ctx);

        public override void BeginEffect(UniRandom random)
        {

            EffectActive = true;
            s_random = random;
            s_OnRingCollected += CollectRing;
            GameEvents.OnEnemyDeath += CollectBlood;
            GameObject prefab = UT2Assets.GetAsset<GameObject>("Assets/Telephone 2/Currencies/CurrencyHUD.prefab");
            _currencyUI = Instantiate(prefab, CanvasController.instance.transform);
        }
        public override int GetEffectCost() => 5;
        protected override void OnDestroy()
        {
            EffectActive = false;
            s_OnRingCollected -= CollectRing;
            GameEvents.OnEnemyDeath -= CollectBlood;
            Destroy(_currencyUI);
        }

        public static void RingCollected()
        {
            s_OnRingCollected?.Invoke();
        }

        private void CollectRing()
        { 
            UT2SaveData.SaveData.Rings++;
            UT2SaveData.MarkDirty();
            CurrencyHUD.Instance.UpdateRingsCounter();
        }

        private readonly HashSet<EnemyType> _machines =
            [
                EnemyType.Drone,
                EnemyType.Streetcleaner,
                EnemyType.Swordsmachine,
                EnemyType.Mindflayer,
                EnemyType.Turret,
                EnemyType.Gutterman,
                EnemyType.Guttertank,
                EnemyType.V2,
                EnemyType.V2Second,
                EnemyType.Centaur,
            ];

        private void CollectBlood(EnemyDeathEvent deathEvent)
        {
            if (!EffectActive || !s_enabled.Value)
                return;

            UT2SaveData.SaveData.Blood += s_random.Next(5, 80);

            if (_machines.Contains(deathEvent.Enemy.enemyType))
            {
                if (deathEvent.Enemy.enemyType == EnemyType.Centaur)
                {
                    UT2SaveData.SaveData.MetalScraps += s_random.Next(40, 280);
                }
                else
                {
                    UT2SaveData.SaveData.MetalScraps += s_random.Next(1, 4);
                }
            }

            UT2SaveData.SaveData.MarketCoins += GetEnemyWorth(deathEvent.Enemy);

            CurrencyHUD.Instance.UpdateBloodCounter();
            CurrencyHUD.Instance.UpdateMetalScrapsCounter();
            CurrencyHUD.Instance.UpdateMarketCoinCounter();

            UT2SaveData.MarkDirty();
        }

        private static int GetEnemyWorth(EnemyIdentifier eid)
        {
            if (eid.dontCountAsKills)
                return 0;

            return eid.enemyType switch
            {
                EnemyType.Filth => 1,
                EnemyType.MaliciousFace => 10,
                EnemyType.Stray => 5,
                EnemyType.Schism => 12,
                EnemyType.Swordsmachine => 35,
                EnemyType.Cerberus => 35,
                EnemyType.Drone => 5,
                EnemyType.Streetcleaner => 10,
                EnemyType.CancerousRodent => 69,
                EnemyType.VeryCancerousRodent => 420,
                EnemyType.HideousMass => 250,
                EnemyType.V2 => 1000,
                EnemyType.Soldier => 15,
                EnemyType.Mindflayer => 75,
                EnemyType.Minos => 350,
                EnemyType.Gabriel => 2500,
                EnemyType.FleshPrison => 5000,
                EnemyType.MinosPrime => 50000,
                EnemyType.Virtue => 180,
                EnemyType.Stalker => 20,
                EnemyType.Mandalore => 6969,
                EnemyType.Sisyphus => 550,
                EnemyType.V2Second => 2500,
                EnemyType.Turret => 150,
                EnemyType.Idol => 100,
                EnemyType.Ferryman => 1250,
                EnemyType.Leviathan => 680,
                EnemyType.GabrielSecond => 5000,
                EnemyType.FleshPanopticon => 7500,
                EnemyType.SisyphusPrime => 75000,
                EnemyType.Mannequin => 60,
                EnemyType.Minotaur => 2500,
                EnemyType.BigJohnator => -10,
                EnemyType.Gutterman => 80,
                EnemyType.Guttertank => 80,
                EnemyType.Centaur => 2000,
                _ => 0,
            };
        }

        #region patches
        [HarmonyPatch(typeof(FishingRodWeapon), nameof(FishingRodWeapon.FishCaughtAndGrabbed))]
        [HarmonyPostfix]
        private static void CatchFish(FishingRodWeapon __instance)
        {
            if (!EffectActive || !s_enabled.Value)
               return;

            UT2SaveData.SaveData.Fish++;
            CurrencyHUD.Instance.UpdateFishCounter();
            UT2SaveData.MarkDirty();
        }

        [HarmonyPatch(typeof(ItemIdentifier), nameof(ItemIdentifier.PickUp))]
        [HarmonyPostfix]
        private static void PickUpPlushie(ItemIdentifier __instance)
        {
            if (!EffectActive || !s_enabled.Value)
                return;

            if (__instance.itemType is ItemType.SkullBlue or ItemType.SkullRed or ItemType.SkullGreen or ItemType.Torch or ItemType.Readable or ItemType.Breakable or ItemType.Soap)
                return;

            UT2SaveData.SaveData.Plushies++;
            CurrencyHUD.Instance.UpdatePlushiesCounter();
            UT2SaveData.MarkDirty();
        }

        [HarmonyPatch(typeof(ItemIdentifier), nameof(ItemIdentifier.PutDown))]
        [HarmonyPostfix]
        private static void PutDownPlushie(ItemIdentifier __instance)
        {
            if (!EffectActive || !s_enabled.Value)
                return;

            if (__instance.itemType is ItemType.SkullBlue or ItemType.SkullRed or ItemType.SkullGreen or ItemType.Torch or ItemType.Readable or ItemType.Breakable or ItemType.Soap)
                return;

            UT2SaveData.SaveData.Plushies--;
            CurrencyHUD.Instance.UpdatePlushiesCounter();
            UT2SaveData.MarkDirty();
        }

        [HarmonyPatch(typeof(StatsManager), nameof(StatsManager.SendInfo))]
        [HarmonyPostfix]
        private static void GainLevelCompleteTrophy()
        {
            if (!EffectActive || !s_enabled.Value)
                return;

            UT2SaveData.SaveData.Trophies++;
            CurrencyHUD.Instance.UpdateTrophiesCounter();
            UT2SaveData.MarkDirty();
        }

        [HarmonyPatch(typeof(Explosion), nameof(Explosion.Start))]
        [HarmonyPostfix]
        private static void PickupGunpowder()
        {
            if (!EffectActive || !s_enabled.Value)
                return;

            UT2SaveData.SaveData.Gunpowder++;
            CurrencyHUD.Instance.UpdateGunpowderCounter();
            UT2SaveData.MarkDirty();
        }
        #endregion
    }
}
