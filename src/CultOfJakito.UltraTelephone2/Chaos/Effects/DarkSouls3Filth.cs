using Configgy;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using HarmonyLib;
using UnityEngine;
using UnityEngine.AI;

namespace CultOfJakito.UltraTelephone2.Chaos.Effects
{
    [HarmonyPatch]
    [RegisterChaosEffect]
    public class DarkSouls3Filth : ChaosEffect
    {
        [Configgable("Chaos/Effects", "Dark Souls 3 Filth")]
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

        [HarmonyPatch(typeof(Zombie), nameof(Zombie.Start)), HarmonyPostfix]
        private static void OnDroneStart(Zombie __instance)
        {
            if (!s_effectActive || !s_enabled.Value)
                return;

            EnemyIdentifier eid = __instance.GetComponent<EnemyIdentifier>();
            if (eid == null)
                return;

            if (eid.enemyType != EnemyType.Filth)
                return;

            if (__instance.TryGetComponent<DarkSouls3FilthObject>(out DarkSouls3FilthObject filf))
                return;

            filf = __instance.gameObject.AddComponent<DarkSouls3FilthObject>();
            filf.filth = __instance;
        }
    }

    public class DarkSouls3FilthObject : MonoBehaviour
    {
        public Zombie filth;

        private float timeUntilTeleport = 1.2f;
        private float teleportDelay = 1.2f;

        private void Update()
        {
            if(filth.eid.dead)
            {
                Destroy(this);
                return;
            }

            if(timeUntilTeleport > 0f)
            {
                timeUntilTeleport = Mathf.Max(0f, timeUntilTeleport - Time.deltaTime);
                return;
            }

            if (filth.gc.onGround)
            {
                timeUntilTeleport = teleportDelay;

                if (NavMesh.SamplePosition(CameraController.Instance.transform.position, out NavMeshHit hit, 4f, -1))
                {
                    filth.nma.Warp(hit.position);
                }
            }
        }
    }
}

