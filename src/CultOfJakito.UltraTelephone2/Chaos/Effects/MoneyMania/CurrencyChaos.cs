using System;
using System.Collections.Generic;
using System.Text;
using Configgy;
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

        public static event Action OnRingCollected;

        public override bool CanBeginEffect(ChaosSessionContext ctx) => s_enabled.Value && base.CanBeginEffect(ctx);
        public override void BeginEffect(UniRandom random)
        {
            // TODO: make ui elements for the currency types and instante them here

            s_effectActive = true;
            s_random = random;
            OnRingCollected += CollectRing;
            GameEvents.OnEnemyDeath += CollectBlood;
        }
        public override int GetEffectCost() => 5;
        protected override void OnDestroy()
        {
            s_effectActive = false;
            OnRingCollected -= CollectRing;
            GameEvents.OnEnemyDeath -= CollectBlood;
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

            UT2SaveData.MarkDirty();
        }

        // Patches

        [HarmonyPatch(typeof(FishingRodWeapon), nameof(FishingRodWeapon.FishCaughtAndGrabbed))]
        private static void CatchFish(FishingRodWeapon __instance)
        {
            if (!s_effectActive || !s_enabled.Value)
                return;

            UT2SaveData.SaveData.Fish++;
            UT2SaveData.MarkDirty();
        }

        [HarmonyPatch(typeof(ItemIdentifier), nameof(ItemIdentifier.PickUp))]
        private static void PickUpPlushie(ItemIdentifier __instance)
        {
            if (!s_effectActive || !s_enabled.Value)
                return;

            if (__instance.itemType is ItemType.SkullBlue or ItemType.SkullRed or ItemType.SkullGreen)
                return;

            UT2SaveData.SaveData.Plushies++;
            UT2SaveData.MarkDirty();
        }

        [HarmonyPatch(typeof(ItemIdentifier), nameof(ItemIdentifier.PutDown))]
        private static void PutDownPlushie(ItemIdentifier __instance)
        {
            if (!s_effectActive || !s_enabled.Value)
                return;

            if (__instance.itemType is ItemType.SkullBlue or ItemType.SkullRed or ItemType.SkullGreen)
                return;

            UT2SaveData.SaveData.Plushies--;
            UT2SaveData.MarkDirty();
        }
    }
}
