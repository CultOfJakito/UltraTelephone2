using System;
using System.Collections.Generic;
using System.Text;
using Configgy;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Chaos.Effects
{
    [RegisterChaosEffect]
    public class LightsOut : ChaosEffect
    {
        [Configgable("Chaos/Effects", "Lights Out")]
        private static ConfigToggle s_enabled = new ConfigToggle(true);

        private List<GamerLight> instances = new List<GamerLight>();

        private Light flashLight;
        public override void BeginEffect(UniRandom random)
        {
            s_enabled.OnValueChanged += OnEnabledChanged;
            effectActive = true;

            flashLight = CameraController.Instance.gameObject.AddComponent<Light>();
            flashLight.type = LightType.Spot;
            flashLight.range = 30f;
            flashLight.spotAngle = 80f;
            flashLight.intensity = 2f;

            SlowTickGetLights();
        }

        private bool effectActive = false;
        private float timeUntilNextColorChange = 0f;


        private void OnEnabledChanged(bool enabled)
        {
            if (effectActive != enabled && !enabled)
            {
                for (int i = 0; i < instances.Count; i++)
                {
                    instances[i]?.Reset();
                }
            }

            effectActive = enabled;
        }

        private void SlowTickGetLights()
        {
            Light[] lights = UnityEngine.Resources.FindObjectsOfTypeAll<Light>();
            for (int i = 0; i < lights.Length; i++)
                ProcessLight(lights[i]);

            Invoke(nameof(SlowTickGetLights), 5f);
        }

        private void LateUpdate()
        {
            if (!effectActive)
                return;

            for (int i = 0; i < instances.Count; i++)
            {
                if (instances[i] == null)
                    continue;

                if (instances[i].Light == null)
                    continue;

                instances[i].Light.enabled = false;
                RenderSettings.ambientLight = Color.black;
                RenderSettings.fog = false;
            }
        }

        public override bool CanBeginEffect(ChaosSessionContext ctx)
        {
            if (!s_enabled.Value || !base.CanBeginEffect(ctx))
                return false;

            if (ctx.ContainsEffect<GamerLights>())
                return false;

            if (ctx.ContainsEffect<QuietMountain>())
                return false;

            return true;
        }

        public override int GetEffectCost()
        {
            return 1;
        }

        protected override void OnDestroy()
        {
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
            if(light == NewMovement.Instance.pointLight || light == flashLight)
                return;

            GamerLight gLight = new GamerLight(light);
            instances.Add(gLight);
        }
    }
}
