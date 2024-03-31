using System;
using System.Collections.Generic;
using System.Text;
using Configgy;
using CultOfJakito.UltraTelephone2.Data;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using CultOfJakito.UltraTelephone2.Events;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Chaos.Effects.CurrencyChaos
{

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
        }

        private void CollectBlood(EnemyDeathEvent deathEvent)
        {
            if (!s_effectActive || !s_enabled.Value)
                return;

            UT2SaveData.SaveData.Blood += s_random.Next(5, 80);
        }

        // Patches

    }
}
