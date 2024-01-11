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
    [HarmonyPatch]
    [RegisterChaosEffect]
    public class IdolsForEveryone : ChaosEffect
    {
        [Configgable("Hydra/Chaos", "Idols For All!")]
        private static ConfigToggle s_enabled = new ConfigToggle(true);

        private static bool s_effectActive = false;
        private System.Random random;

        public override void BeginEffect(System.Random random)
        {
            this.random = random;
            s_effectActive = true;
        }


        [HarmonyPatch(typeof(EnemyIdentifier), "Awake"), HarmonyPostfix]
        public static void OnEnemySpawn(EnemyIdentifier __instance)
        {
            if (!s_enabled.Value || !s_effectActive)
                return;

            if (__instance.dead)
                return;

            //Dont spawn idols for idols or centaur ffs
            if (__instance.enemyType == EnemyType.Idol || __instance.enemyType == EnemyType.Centaur)
                return;

            Vector3 pos = __instance.transform.position;
            Transform parent = __instance.transform.parent;

            if (parent == null)
                return;

            //spawn an idol
            UKPrefabs.Idol.LoadObjectAsync((s, r) =>
            {
                if (__instance == null || parent == null || s != UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
                    return;

                GameObject newIdol = GameObject.Instantiate(r, pos, Quaternion.identity);
                newIdol.transform.SetParent(parent, true);
                Idol idol = newIdol.GetComponent<Idol>();
                idol.ChangeOverrideTarget(__instance);
            });

        }

        public override int GetEffectCost()
        {
            return 5;
        }

        public override bool CanBeginEffect(ChaosSessionContext ctx)
        {
            if (!s_enabled.Value)
                return false;

            return base.CanBeginEffect(ctx);
        }

        private void OnDestroy()
        {
            s_effectActive = false;
        }
    }
}
