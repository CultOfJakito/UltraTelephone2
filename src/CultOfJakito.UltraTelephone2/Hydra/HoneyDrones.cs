using System;
using System.Collections.Generic;
using System.Text;
using Configgy;
using CultOfJakito.UltraTelephone2.Assets;
using HarmonyLib;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Hydra
{
    [HarmonyPatch]
    public static class HoneyDrones
    {
        [Configgable("Patches", "Bee Drones")]
        private static ConfigToggle s_enabled = new ConfigToggle(true);

        [HarmonyPatch(typeof(Drone), nameof(Drone.Start)), HarmonyPostfix]
        public static void OnStart(Drone __instance)
        {
            if (!s_enabled.Value)
                return;

            MakeDroneBee(__instance);
        }

        private static void MakeDroneBee(Drone __instance)
        {
            if (!__instance.TryGetComponent<EnemyIdentifier>(out EnemyIdentifier eid))
                return;

            if (eid.enemyType != EnemyType.Drone)
                return;

            AudioSource audioSource = __instance.gameObject.AddComponent<AudioSource>();
            audioSource.clip = HydraAssets.BeeAudioLoop;
            audioSource.loop = true;
            audioSource.volume = 1f;
            audioSource.spatialBlend = 1f;
            audioSource.minDistance = 10f;
            audioSource.maxDistance = 50f;
            audioSource.outputAudioMixerGroup = AudioMixerController.Instance.allSound.outputAudioMixerGroup;
            audioSource.Play();
        }
    }
}
