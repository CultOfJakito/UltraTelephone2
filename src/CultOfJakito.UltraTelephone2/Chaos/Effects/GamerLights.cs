using Configgy;
using CultOfJakito.UltraTelephone2.Chaos;
using CultOfJakito.UltraTelephone2.Data;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Hydra.Chaos
{
    [RegisterChaosEffect]
    public class GamerLights : ChaosEffect
    {
        [Configgable("Chaos/Effects/Gamer Lights", "Gamer Lights")]
        private static ConfigToggle s_enabled = new ConfigToggle(true);

        [Configgable("Chaos/Effects/Gamer Lights", "Color Fade Speed")]
        private static ConfigInputField<float> s_lightChangeSpeed = new ConfigInputField<float>(0.3f, (v) =>
        {
            return v > 0f;
        });

        [Configgable("Chaos/Effects/Gamer Lights", "Color Change Delay")]
        private static ConfigInputField<float> s_colorChangeDelay = new ConfigInputField<float>(1f, (v) =>
        {
            return v > 0f;
        });

        [Configgable("Chaos/Effects/Gamer Lights", "Unified Color")]
        private static ConfigToggle s_unifiedColor = new ConfigToggle(false);

        private List<GamerLight> instances = new List<GamerLight>();
        
        private UniRandom s_rng;

        public override void BeginEffect(UniRandom random)
        {
            s_rng = random;
            s_enabled.OnValueChanged += OnEnabledChanged;
            effectActive = true;

            SlowTickGetLights();
        }

        private bool effectActive = false;
        private float timeUntilNextColorChange = 0f;


        private void OnEnabledChanged(bool enabled)
        {
            if(effectActive != enabled && !enabled)
            {
                for(int i = 0; i < instances.Count; i++)
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

            if(timeUntilNextColorChange > 0f)
            {
                timeUntilNextColorChange = Mathf.Max(0f, timeUntilNextColorChange - Time.deltaTime);
            }
            else
            {
                Color color = s_rng.Color(1f,1f,1f);

                timeUntilNextColorChange = s_colorChangeDelay.Value;
                for (int i = 0; i < instances.Count; i++)
                {
                    if (instances[i] == null)
                        continue;

                    if (!s_unifiedColor.Value)
                    {
                        instances[i].NewColor(s_rng.Color(1f, 1f, 1f));
                    }else
                    {
                        instances[i].NewColor(color);
                    }
                }
            }

            for (int i = 0; i < instances.Count; i++)
            {
                instances[i]?.Tick(s_lightChangeSpeed.Value*Time.deltaTime);
            }
        }

        public override bool CanBeginEffect(ChaosSessionContext ctx) => s_enabled.Value && base.CanBeginEffect(ctx);

        public override int GetEffectCost()
        {
            return 1;
        }

        public override void Dispose()
        {
            s_enabled.OnValueChanged -= OnEnabledChanged;
            base.Dispose();
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

            GamerLight gLight = new GamerLight(light);
            instances.Add(gLight);
        }

    }

    public class GamerLight
    {
        public Light Light;

        private Color originalColor;
        private Color targetColor;
        private Color currentColor;

        public GamerLight(Light light)
        {
            this.Light = light;
            originalColor = light.color;
            currentColor = originalColor;
            targetColor = originalColor;
        }

        public void NewColor(Color color)
        {
            targetColor = color;
        }

        public void Tick(float delta)
        {
            if (Light == null)
                return;

            Color newColor = currentColor;
            newColor.r = Mathf.MoveTowards(newColor.r, targetColor.r, delta);
            newColor.g = Mathf.MoveTowards(newColor.g, targetColor.g, delta);
            newColor.b = Mathf.MoveTowards(newColor.b, targetColor.b, delta);
            currentColor = newColor;
            Light.color = currentColor;
        }

        public void Reset()
        {
            if (Light == null)
                return;

            Light.color = originalColor;
        }
    }
}
