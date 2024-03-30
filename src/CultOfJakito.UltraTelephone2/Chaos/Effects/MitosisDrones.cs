using System;
using System.Collections.Generic;
using System.Text;
using Configgy;
using CultOfJakito.UltraTelephone2.Assets;
using CultOfJakito.UltraTelephone2.Data;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using CultOfJakito.UltraTelephone2.Hydra;
using HarmonyLib;
using Steamworks;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Chaos.Effects
{
    [RegisterChaosEffect]
    [HarmonyPatch]
    public class MitosisDrones : ChaosEffect
    {
        [Configgable("Chaos/Effects/Mitosis Drones", "Drone Mitosis")]
        private static ConfigToggle s_enabled = new ConfigToggle(true);

        [Configgable("Chaos/Effects/Mitosis Drones", "Mitosis Speed")]
        private static ConfigInputField<float> s_mitosisSpeed = new ConfigInputField<float>(2.5f);

        [Configgable("Chaos/Effects/Mitosis Drones", "Max Mitosis Generations")]
        private static ConfigInputField<int> s_mitosisMax = new ConfigInputField<int>(5);

        private static bool s_effectActive = false;

        public override void BeginEffect(UniRandom random)
        {
            s_effectActive = true;
        }

        public override bool CanBeginEffect(ChaosSessionContext ctx) => s_enabled.Value && base.CanBeginEffect(ctx);

        public override int GetEffectCost()
        {
            return 6;
        }

        public override void Dispose()
        {
            s_effectActive = false;
            base.Dispose();
        }

        [HarmonyPatch(typeof(Drone), nameof(Drone.Start)), HarmonyPostfix]
        private static void OnDroneStart(Drone __instance)
        {
            if (!s_effectActive || !s_enabled.Value)
                return;

            EnemyIdentifier eid = __instance.GetComponent<EnemyIdentifier>();
            if (eid == null)
                return;

            if (eid.enemyType != EnemyType.Drone && eid.enemyType != EnemyType.Virtue)
                return;

            //Add the component if it doesn't exist
            if (!__instance.TryGetComponent<MitosisObject>(out MitosisObject mitosis))
            {
                mitosis = __instance.gameObject.AddComponent<MitosisObject>();
                mitosis.TimeToMitosis = s_mitosisSpeed.Value;
                mitosis.MitosisDelay = s_mitosisSpeed.Value;
                mitosis.MaxGeneration = s_mitosisMax.Value;
            }
        }
    }

    public class MitosisObject : MonoBehaviour
    {
        public int Generation;
        public int MaxGeneration = 5;
        public float TimeToMitosis;
        public float MitosisDelay;

        public Func<bool> shouldKeepDuping;

        private void Update()
        {
            if (Generation >= MaxGeneration)
                return;

            if(shouldKeepDuping != null && !shouldKeepDuping())
            {
                Destroy(this);
                return;
            }

            if(TimeToMitosis > 0f)
            {
                TimeToMitosis = Mathf.Max(0f, TimeToMitosis - Time.deltaTime);
                return;
            }

            TimeToMitosis = MitosisDelay;
            GameObject clone = GameObject.Instantiate(gameObject, transform.position, transform.rotation, transform.parent);
            clone.GetComponent<MitosisObject>().Generation = Generation + 1;
        }
    }
}
