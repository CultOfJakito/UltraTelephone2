using System;
using System.Collections.Generic;
using System.Text;
using Configgy;
using CultOfJakito.UltraTelephone2.Assets;
using CultOfJakito.UltraTelephone2.Data;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using CultOfJakito.UltraTelephone2.Fun.Herobrine;
using CultOfJakito.UltraTelephone2.Util;
using UnityEngine;
using UnityEngine.AI;

namespace CultOfJakito.UltraTelephone2.Chaos.Effects
{
    [RegisterChaosEffect]
    public class LightsOut : ChaosEffect
    {
        [Configgable("Chaos/Effects", "Lights Out")]
        private static ConfigToggle s_enabled = new ConfigToggle(true);

        private List<GamerLight> instances = new List<GamerLight>();
        private UniRandom random;

        private Light flashLight;
        public override void BeginEffect(UniRandom random)
        {
            this.random = random;
            s_enabled.OnValueChanged += OnEnabledChanged;
            effectActive = true;

            flashLight = CameraController.Instance.gameObject.AddComponent<Light>();
            flashLight.type = LightType.Spot;
            flashLight.range = 30f;
            flashLight.spotAngle = 80f;
            flashLight.intensity = 2f;

            Herobrine.MoreFrequentHerobrine = true;

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
            if(random.Chance(0.035f))
            {
                HudMessageReceiver.Instance.SendHudMessage("Something wicked this way comes....");

                if (random.Bool())
                {
                    if(NavUtils.TryGetRandomPointOnNavMesh(out NavMeshHit hit))
                    {
                        GameObject wickedObj = GameObject.Instantiate(UkPrefabs.SomethingWicked.GetObject(), hit.position, Quaternion.identity);
                        string randomName = random.SelectRandom(UT2TextFiles.EnemyNamesFile.TextList);
                        Transform patrolPoint = new GameObject($"{randomName}'s Patrol Point").transform;
                        patrolPoint.transform.position = hit.position;
                        wickedObj.name = $"WICKED ({randomName})";

                        //Fix the error where the wicked doesn't have a patrol points
                        Wicked wicked = wickedObj.GetComponent<Wicked>();
                        Transform playerTF = CameraController.Instance.transform;
                        wicked.patrolPoints = new Transform[1] { patrolPoint };
                        wicked.targetPoint = playerTF;
                    }
                }
            }

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
            Herobrine.MoreFrequentHerobrine = false;
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
