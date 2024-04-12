using Configgy;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using CultOfJakito.UltraTelephone2.Fun.Glungus;
using HarmonyLib;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Chaos.Effects
{
    [RegisterChaosEffect]
    [HarmonyPatch]
    public class GlungusFriends : ChaosEffect
    {
        [Configgable("Chaos/Effects/Glungus Army", "Glungus Army")]
        private static ConfigToggle s_enabled = new ConfigToggle(true);

        [Configgable("Chaos/Effects/Glungus Army", "Spawn Rate")]
        private static ConfigInputField<float> s_glungusSpawnRate = new ConfigInputField<float>(5f);

        [Configgable("Chaos/Effects/Glungus Army", "Spawn Chance")]
        private static ConfigInputField<float> s_glungusSpawnChance = new ConfigInputField<float>(0.65f);

        private  UniRandom rng;

        private float timeUntilNextSpawn = 0f;

        private bool effectActive = false;

        public override void BeginEffect(UniRandom random)
        {
            rng = random;
            effectActive = true;
        }

        private void Update()
        {
            if (!effectActive)
                return;

            if(timeUntilNextSpawn  > 0f)
            {
                timeUntilNextSpawn = Mathf.Max(0f, timeUntilNextSpawn - Time.deltaTime);
                return;
            }

            timeUntilNextSpawn = s_glungusSpawnRate.Value;
            if(rng.Chance(s_glungusSpawnChance.Value))
            {
                GlungusManager.SpawnGlungus(NewMovement.Instance.transform.position);
            }
        }

        public override bool CanBeginEffect(ChaosSessionContext ctx) => s_enabled.Value && GlungusManager.S_Enabled.Value && base.CanBeginEffect(ctx);

        public override int GetEffectCost()
        {
            return 2;
        }

        protected override void OnDestroy() {}
    }
}
