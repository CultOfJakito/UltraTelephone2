﻿using Configgy;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using HarmonyLib;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Chaos.Effects
{
    [HarmonyPatch]
    [RegisterChaosEffect]
    public class ArmoredVirtues : ChaosEffect
    {
        [Configgable("Chaos/Effects", "Armored Virtues")]
        private static ConfigToggle s_enabled = new ConfigToggle(true);

        private static bool s_effectActive = false;

        public override void BeginEffect(UniRandom random)
        {
            s_effectActive = true;
        }

        public override bool CanBeginEffect(ChaosSessionContext ctx) => s_enabled.Value && base.CanBeginEffect(ctx);

        public override int GetEffectCost()
        {
            return 2;
        }

        protected override void OnDestroy() => s_effectActive = false;

        [HarmonyPatch(typeof(Drone), "Start"), HarmonyPostfix]
        private static void OnDroneStart(Drone __instance)
        {
            if (!s_enabled.Value || !s_effectActive)
                return;

            if(!__instance.TryGetComponent<EnemyIdentifier>(out EnemyIdentifier eid))
                return;

            if (eid.enemyType != EnemyType.Virtue)
                return;

            EnemyIdentifierIdentifier hitbox = __instance.GetComponentsInChildren<EnemyIdentifierIdentifier>().Where(x=>x.name == "Sphere").FirstOrDefault();
            if (hitbox == null)
                return;

            //Shrink hitbox and make it armored
            hitbox.gameObject.tag = "Armor";
            hitbox.gameObject.layer = 26;
            hitbox.gameObject.transform.localScale = Vector3.one * 3f;
        }
    }
}
