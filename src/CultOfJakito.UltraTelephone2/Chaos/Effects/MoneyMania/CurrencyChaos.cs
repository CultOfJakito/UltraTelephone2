using System;
using System.Collections.Generic;
using System.Text;
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
        [Configgable("Chaos/Effects/", "Currency Chaos")]
        private static ConfigToggle s_enabled = new ConfigToggle(true);

        private static bool s_effectActive = false;
        public static bool EffectActive { get { return s_effectActive; } }

        private static UniRandom s_random;

        private GameObject _currencyUI;

        public static event Action OnRingCollected;

        public override bool CanBeginEffect(ChaosSessionContext ctx) => s_enabled.Value && base.CanBeginEffect(ctx);
        public override void BeginEffect(UniRandom random)
        {

            s_effectActive = true;
            s_random = random;
            OnRingCollected += CollectRing;
            GameEvents.OnEnemyDeath += CollectBlood;
            _currencyUI = UT2Assets.GetAsset<GameObject>("");
            _currencyUI = Instantiate(_currencyUI, CanvasController.instance.transform);

        }
        public override int GetEffectCost() => 5;
        protected override void OnDestroy()
        {
            s_effectActive = false;
            OnRingCollected -= CollectRing;
            GameEvents.OnEnemyDeath -= CollectBlood;
            Destroy(_currencyUI);
        }

        public static void InvokeRingCollected()
        {
            OnRingCollected?.Invoke();
        }

        private void CollectRing()
        {
            // this check technically isn't needed, but in case i've screwed something up...
            if (!s_effectActive || !s_enabled.Value)
                return;

            UT2SaveData.SaveData.Rings++;
            UT2SaveData.MarkDirty();
        }

        private HashSet<EnemyType> _machines =
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
            if (!s_effectActive || !s_enabled.Value)
                return;

            UT2SaveData.SaveData.Blood += s_random.Next(5, 80);

            if (_machines.Contains(deathEvent.Enemy.enemyType))
            {
                if (deathEvent.Enemy.enemyType == EnemyType.Centaur)
                {
                    UT2SaveData.SaveData.MetalScraps += s_random.Next(40, 280);
                    UT2SaveData.MarkDirty();
                    return;
                }

                UT2SaveData.SaveData.MetalScraps += s_random.Next(1, 4);
            }

            // Todo: depedning on enemy killed, gain ultramarket coins


            CurrencyHUD.Instance.UpdateBloodCounter();
            CurrencyHUD.Instance.UpdateMetalScrapsCounter();
            CurrencyHUD.Instance.UpdateMarketCoinCounter();

            UT2SaveData.MarkDirty();
        }

        // Patches

        [HarmonyPatch(typeof(FishingRodWeapon), nameof(FishingRodWeapon.FishCaughtAndGrabbed))]
        [HarmonyPostfix]
        private static void CatchFish(FishingRodWeapon __instance)
        {
            if (!s_effectActive || !s_enabled.Value)
                return;

            UT2SaveData.SaveData.Fish++;

            CurrencyHUD.Instance.UpdateFishCounter();

            UT2SaveData.MarkDirty();
        }

        [HarmonyPatch(typeof(ItemIdentifier), nameof(ItemIdentifier.PickUp))]
        [HarmonyPostfix]
        private static void PickUpPlushie(ItemIdentifier __instance)
        {
            if (!s_effectActive || !s_enabled.Value)
                return;

            if (__instance.itemType is ItemType.SkullBlue or ItemType.SkullRed or ItemType.SkullGreen)
                return;

            UT2SaveData.SaveData.Plushies++;

            CurrencyHUD.Instance.UpdatePlushiesCounter();

            UT2SaveData.MarkDirty();
        }

        [HarmonyPatch(typeof(ItemIdentifier), nameof(ItemIdentifier.PutDown))]
        [HarmonyPostfix]
        private static void PutDownPlushie(ItemIdentifier __instance)
        {
            if (!s_effectActive || !s_enabled.Value)
                return;

            if (__instance.itemType is ItemType.SkullBlue or ItemType.SkullRed or ItemType.SkullGreen)
                return;

            UT2SaveData.SaveData.Plushies--;

            CurrencyHUD.Instance.UpdatePlushiesCounter();

            UT2SaveData.MarkDirty();
        }

        [HarmonyPatch(typeof(StatsManager), nameof(StatsManager.SendInfo))]
        [HarmonyPostfix]
        private static void GainLevelCompleteTrophy()
        {
            if (!s_effectActive || !s_enabled.Value)
                return;

            UT2SaveData.SaveData.Trophies++;

            CurrencyHUD.Instance.UpdateTrophiesCounter();

            UT2SaveData.MarkDirty();
        }

        [HarmonyPatch(typeof(Explosion), nameof(Explosion.Start))]
        [HarmonyPostfix]
        private static void PickupGunpowder()
        {
            if (!s_effectActive || !s_enabled.Value)
                return;

            UT2SaveData.SaveData.Gunpowder++;

            CurrencyHUD.Instance.UpdateGunpowderCounter();

            UT2SaveData.MarkDirty();
        }
    }
}
