using System;
using System.Collections.Generic;
using System.Text;
using Configgy;
using CultOfJakito.UltraTelephone2.Chaos;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using HarmonyLib;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2
{
    [RegisterChaosEffect]
    [HarmonyPatch]
    public class DoorStuck : ChaosEffect
    {

        [Configgable("Hydra/Chaos", "Door Stuck")]
        private static ConfigToggle s_enabled = new ConfigToggle(true);

        static bool s_effectActive = false;
        static System.Random random;

        public override void BeginEffect(System.Random rand)
        {
            random = rand;
            s_effectActive = true;
        }

        [HarmonyPatch(typeof(Door), (nameof(Door.Open))), HarmonyPostfix]
        public static void OnDoorOpen(Door __instance)
        {
            if (!s_effectActive || !s_enabled.Value)
                return;

            if (__instance.TryGetComponent<DoorJammer>(out DoorJammer jammer))
                return;

            jammer = __instance.gameObject.AddComponent<DoorJammer>();
            jammer.Door = __instance;
            jammer.JamOnOpen = true;
            jammer.JamOnPercent = ((float)random.NextDouble() / 5f);
            jammer.UnjamAfterSeconds = ((float)random.NextDouble() * 5f)+3f;
        }

        [HarmonyPatch(typeof(BigDoor), (nameof(BigDoor.Open))), HarmonyPostfix]
        public static void OnBigDoorOpen(BigDoor __instance)
        {
            if (!s_effectActive || !s_enabled.Value)
                return;

            if (__instance.TryGetComponent<DoorJammer>(out DoorJammer jammer))
                return;

            jammer = __instance.gameObject.AddComponent<DoorJammer>();
            jammer.BigDoor = __instance;
            jammer.JamOnOpen = true;
            jammer.JamOnPercent = ((float)random.NextDouble() / 5f);
            jammer.UnjamAfterSeconds = ((float)random.NextDouble() * 5f) + 3f;

        }

        public override bool CanBeginEffect(ChaosSessionContext ctx)
        {
            if (!s_enabled.Value)
                return false;

            return base.CanBeginEffect(ctx);
        }

        public override int GetEffectCost()
        {
            return 4;
        }

        private void OnDestroy()
        {
            s_effectActive = false;
        }
    }

    public class DoorJammer : MonoBehaviour
    {
        public Door Door;
        public BigDoor BigDoor;
        public float JamOnPercent;
        public bool JamOnOpen;
        public float UnjamAfterSeconds = 5f;

        private bool didJam;


        private void Start()
        {
            if (Door != null)
                Door.onFullyOpened.AddListener(() =>
                {
                    didJam = false;
                });

            if(BigDoor != null)
            {
                Door door = BigDoor.GetComponentInParent<Door>();
                if(door != null)
                    door.onFullyOpened.AddListener(() =>
                    {
                        didJam = false;
                    });
            }
        }

        private void Update()
        {
            DoorUpdate();
            BigDoorUpdate();
        }

        private void BigDoorUpdate()
        {
            if (BigDoor == null || didJam)
                return;

            if (JamOnOpen == BigDoor.open)
            {
                Vector3 rot = BigDoor.transform.localEulerAngles;
                float valueTraveled = 0f;

                if (JamOnOpen)
                    valueTraveled = MathUtils.InverseLerpVector3(BigDoor.origRotation.eulerAngles, BigDoor.openRotation, rot);
                else
                    valueTraveled = MathUtils.InverseLerpVector3(BigDoor.openRotation, BigDoor.origRotation.eulerAngles, rot);

                if (valueTraveled > JamOnPercent)
                {
                    Jam();
                }
            }
        }

        private void DoorUpdate()
        {
            if (Door == null || didJam)
                return;

            if (JamOnOpen == Door.open)
            {
                Vector3 pos = Door.transform.localPosition;
                float valueTraveled = 0f;

                if (JamOnOpen)
                    valueTraveled = MathUtils.InverseLerpVector3(Door.closedPos, Door.openPos, pos);
                else
                    valueTraveled = MathUtils.InverseLerpVector3(Door.openPos, Door.closedPos, pos);

                if (valueTraveled > JamOnPercent)
                {
                    Jam();
                }
            }
        }

        private void Jam()
        {
            //Door stuck.
            didJam = true;

            if(Door != null)
                Door.enabled = false;

            if(BigDoor != null)
                BigDoor.enabled = false;

            Invoke(nameof(ReleaseJam), UnjamAfterSeconds);
        }

        private void ReleaseJam()
        {
            if(Door != null)
                Door.enabled = true;

            if(BigDoor != null)
                BigDoor.enabled = true;
        }
    }
}
