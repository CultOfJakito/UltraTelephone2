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
    public static class StreetcleanerBubbles
    {
        [Configgable("Patches", "Street Cleaner Bubbles")]
        private static ConfigToggle s_enabled = new ConfigToggle(true);


        [HarmonyPatch(typeof(Streetcleaner), nameof(Streetcleaner.Start)), HarmonyPostfix]
        public static void OnStart(Streetcleaner __instance)
        {
            if (!s_enabled.Value)
                return;

            FireZone fireZone = __instance.GetComponentInChildren<FireZone>(true);
            if (fireZone == null)
                return;


            ParticleSystem ps = fireZone.GetComponentInParent<ParticleSystem>();
            if (ps == null)
                return;



            ParticleSystem.MainModule main = ps.main;
            main.startSize = new ParticleSystem.MinMaxCurve(0.1f, 1f);
            main.gravityModifier = -2f;


            ParticleSystem.EmissionModule em = ps.emission;
            em.rateOverTime = 150;

            ParticleSystemRenderer psr = ps.GetComponent<ParticleSystemRenderer>();
            if (psr == null)
                return;

            psr.material = UkPrefabs.BubbleMaterial.GetObject();
            AudioDistortionFilter dist = ps.GetComponentInParent<AudioDistortionFilter>();
            dist.enabled = false;
            AudioSource src = dist.GetComponent<AudioSource>();
            src.clip = UkPrefabs.BubbleLoopClip.GetObject();

        }
    }
}
