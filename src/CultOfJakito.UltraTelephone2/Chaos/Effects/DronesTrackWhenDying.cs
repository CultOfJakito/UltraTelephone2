using System;
using System.Collections.Generic;
using System.Text;
using Configgy;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using HarmonyLib;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Chaos.Effects
{
    [HarmonyPatch]
    [RegisterChaosEffect]
    public class DronesTrackWhenDying : ChaosEffect
    {
        [Configgable("Chaos/Effects", "Drones Track When Dying")]
        private static ConfigToggle s_enabled = new ConfigToggle(true);

        public override void BeginEffect(UniRandom random)
        {
            s_effectActive = true;
        }

        private static bool s_effectActive = false;

        public override bool CanBeginEffect(ChaosSessionContext ctx) => s_enabled.Value && base.CanBeginEffect(ctx);

        public override int GetEffectCost()
        {
            return 2;
        }

        protected override void OnDestroy() => s_effectActive = false;


        [HarmonyPatch(typeof(Drone), nameof(Drone.Death)), HarmonyPrefix]
        private static void OnStart(Drone __instance)
        {
            if (!s_enabled.Value || !s_effectActive || __instance.crashing)
                return;

            EnemyIdentifier eid = __instance.GetComponent<EnemyIdentifier>();
            if (eid == null)
                return;

            if (eid.enemyType != EnemyType.Drone)
                return;

            if(!__instance.TryGetComponent<BehaviourRelay>(out BehaviourRelay relay))
            {
                relay = __instance.gameObject.AddComponent<BehaviourRelay>();
                relay.OnFixedUpdate += (g) =>
                {
                    Vector3 direction = CameraController.Instance.transform.position - __instance.transform.position;

                    //redirect velocity
                    __instance.rb.velocity = direction.normalized * __instance.rb.velocity.magnitude;
                };
            }

            if(!__instance.TryGetComponent<AlwaysLookAtCamera>(out AlwaysLookAtCamera al))
            {
                al = __instance.gameObject.AddComponent<AlwaysLookAtCamera>();
                al.useXAxis = true;
                al.useYAxis = true;
                al.useZAxis = true;
            }

        }

    }
}
