using Configgy;
using CultOfJakito.UltraTelephone2.Assets;
using HarmonyLib;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Patches
{
    [HarmonyPatch]
    public static class EnemySquishSound
    {
        [Configgable("Patches", "Maurice Splat Noise")]
        private static ConfigToggle s_enabled = new ConfigToggle(true);

        /// <summary>
        /// </summary>
        /// <param name="__state">If enemy is dead</param>
        [HarmonyPatch(typeof(EnemyIdentifier), nameof(EnemyIdentifier.FallOnEnemy)), HarmonyPrefix]
        public static void PreSplat(EnemyIdentifier eid, out bool __state)
        {
            __state = false;
            if (!s_enabled.Value)
                return;

            if (eid == null)
                return;

            __state = eid.dead;
        }


        [HarmonyPatch(typeof(EnemyIdentifier), nameof(EnemyIdentifier.FallOnEnemy)), HarmonyPostfix]
        public static void PostSplat(EnemyIdentifier eid, bool __state)
        {
            if (!s_enabled.Value)
                return;

            if (eid == null)
                return;

            if (!__state && eid.dead)
            {
                //splat noise
                Vector3 position = eid.transform.position;

                AudioClip clip = HydraAssets.SplatSound;
                if (clip == null)
                    return;

                clip.PlaySound(position);
            }
        }

    }
}
