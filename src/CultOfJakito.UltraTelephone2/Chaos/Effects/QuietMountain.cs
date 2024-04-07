using System;
using System.Collections.Generic;
using System.Text;
using Configgy;
using CultOfJakito.UltraTelephone2.Assets;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using CultOfJakito.UltraTelephone2.Fun.FakePBank;
using CultOfJakito.UltraTelephone2.Fun.Herobrine;
using HarmonyLib;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Chaos.Effects
{
    [RegisterChaosEffect]
    [HarmonyPatch]
    public class QuietMountain : ChaosEffect
    {
        [Configgable("Chaos/Effects", "Quiet Mountain")]
        private static ConfigToggle s_enabled = new ConfigToggle(true);

        private List<GamerLight> instances = new List<GamerLight>();
        private static bool s_effectActive = false;

        public override void BeginEffect(UniRandom random)
        {
            Herobrine.MoreFrequentHerobrine = true;
            s_enabled.OnValueChanged += OnEnabledChanged;
            s_effectActive = true;

            MusicManager.Instance.cleanTheme.clip = UkPrefabs.ThroatDrone.GetObject();
            MusicManager.Instance.battleTheme.clip = UkPrefabs.ThroatDrone.GetObject();
            MusicManager.Instance.bossTheme.clip = UkPrefabs.ThroatDrone.GetObject();
            MusicManager.Instance.targetTheme.clip = UkPrefabs.ThroatDrone.GetObject();

            SlowTick();
        }

        private float timeUntilNextColorChange = 0f;


        private void OnEnabledChanged(bool enabled)
        {
            if (s_effectActive != enabled && !enabled)
            {
                for (int i = 0; i < instances.Count; i++)
                {
                    instances[i]?.Reset();
                }
            }

            s_effectActive = enabled;
        }

        private void SlowTick()
        {
            Light[] lights = UnityEngine.Resources.FindObjectsOfTypeAll<Light>();
            for (int i = 0; i < lights.Length; i++)
                ProcessLight(lights[i]);

            MusicManager.Instance.cleanTheme.clip = UkPrefabs.ThroatDrone.GetObject();
            MusicManager.Instance.battleTheme.clip = UkPrefabs.ThroatDrone.GetObject();
            MusicManager.Instance.bossTheme.clip = UkPrefabs.ThroatDrone.GetObject();
            MusicManager.Instance.targetTheme.clip = UkPrefabs.ThroatDrone.GetObject();

            Invoke(nameof(SlowTick), 5f);
        }

        [HarmonyPatch(typeof(ULTRAKILL.Cheats.DisableEnemySpawns), nameof(ULTRAKILL.Cheats.DisableEnemySpawns.DisableArenaTriggers), MethodType.Getter), HarmonyPrefix]
        private static bool FixEnemies(ULTRAKILL.Cheats.DisableEnemySpawns __instance, ref bool __result)
        {
            if (!s_enabled.Value || !s_effectActive)
                return true;

            __result = true;
            return false;

        }


        private void LateUpdate()
        {
            if (!s_effectActive)
                return;

            for (int i = 0; i < instances.Count; i++)
            {
                if (instances[i] == null)
                    continue;

                instances[i].Range = 3f;
                instances[i].Intensity = 0.5f;
                instances[i].Color = Color.white;

                RenderSettings.ambientLight = new Color(0.3f,0.3f,0.3f,0.3f);
                RenderSettings.fog = true;
                RenderSettings.fogMode = FogMode.Linear;
                RenderSettings.fogColor = Color.white;
                RenderSettings.fogStartDistance = 10;
                RenderSettings.fogEndDistance = 30;
            }
        }

        public override bool CanBeginEffect(ChaosSessionContext ctx)
        {
            if (!s_enabled.Value || !base.CanBeginEffect(ctx))
                return false;

            if (ctx.ContainsEffect<GamerLights>())
                return false;

            if (ctx.ContainsEffect<LightsOut>())
                return false;

            return true;
        }

        public override int GetEffectCost()
        {
            return 6;
        }

        protected override void OnDestroy()
        {
            Herobrine.MoreFrequentHerobrine = false;
            s_effectActive = false;
            s_enabled.OnValueChanged -= OnEnabledChanged;
        }

        private HashSet<int> lightsChecked = new HashSet<int>();

        private void ProcessLight(Light light)
        {
            //ignore lights that we already looked at
            if (lightsChecked.Contains(light.GetInstanceID()))
                return;

            lightsChecked.Add(light.GetInstanceID());

            // Ignore lights that are not part of the scene
            if (light.hideFlags == HideFlags.NotEditable || light.hideFlags == HideFlags.HideAndDontSave)
                return;

            //Ignore the player's light..
            if (light == NewMovement.Instance.pointLight)
                return;

            GamerLight gLight = new GamerLight(light);
            instances.Add(gLight);
        }
    }
}
