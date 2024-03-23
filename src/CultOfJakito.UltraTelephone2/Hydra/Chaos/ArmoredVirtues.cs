using Configgy;
using CultOfJakito.UltraTelephone2.Chaos;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using CultOfJakito.UltraTelephone2.Events;
using HarmonyLib;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Hydra
{
    [HarmonyPatch]
    [RegisterChaosEffect]
    public class ArmoredVirtues : ChaosEffect, IEventListener
    {
        [Configgable("Hydra/Chaos", "Armored Virtues")]
        private static ConfigToggle s_enabled = new ConfigToggle(true);

        private static bool s_effectActive = false;

        public override void BeginEffect(UniRandom random)
        {
            s_effectActive = true;
        }

        public bool CanBeginEffect(ChaosSessionContext ctx) => s_enabled.Value && base.CanBeginEffect(ctx);

        public override int GetEffectCost()
        {
            return 3;
        }

        public override void Dispose()
        {
            s_effectActive = false;
            base.Dispose();
        }

        [HarmonyPatch(typeof(Drone), "Start"), HarmonyPostfix]
        private static void OnDroneStart(Drone __instance)
        {
            if (!s_enabled.Value)
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
