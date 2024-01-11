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

        public override void BeginEffect(System.Random random)
        {
            s_effectActive = true;
        }


        [HarmonyPatch(typeof(FinalDoor), (nameof(FinalDoor.Open))), HarmonyPostfix]
        public static void OnFinalDoorOpen(FinalDoor __instance)
        {
            if (!s_enabled.Value || !s_effectActive)
                return;

            __instance.doors.ForEach(x =>
            {
                DoorJammer jammer = x.gameObject.AddComponent<DoorJammer>();
                jammer.Door = x;
                jammer.JamOnOpen = true;
                jammer.JamOnPercent = 0.11f;
            });
        }

        [HarmonyPatch(typeof(Door), (nameof(Door.Open))), HarmonyPostfix]
        public static void OnDoorOpen(Door __instance)
        {
            if (__instance.TryGetComponent<DoorJammer>(out DoorJammer jammer))
                return;

            jammer = __instance.gameObject.AddComponent<DoorJammer>();
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
        public float JamOnPercent;
        public bool JamOnOpen;
        public float UnjamAfterSeconds = 5f;

        private void Update()
        {
            if (Door == null)
                return;

            if(JamOnOpen == Door.open)
            {
                Vector3 pos = Door.transform.localPosition;
                float valueTraveled = 0f;

                if(JamOnOpen)
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
            Door.enabled = false;
            Invoke(nameof(ReleaseJam), UnjamAfterSeconds);
        }

        private void ReleaseJam()
        {
            Door.enabled = true;
            enabled = false;
        }
    }
}
