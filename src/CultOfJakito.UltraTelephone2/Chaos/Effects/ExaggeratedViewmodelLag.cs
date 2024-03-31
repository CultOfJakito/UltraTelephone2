using System;
using System.Collections.Generic;
using System.Text;
using Configgy;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using CultOfJakito.UltraTelephone2.Events;
using CultOfJakito.UltraTelephone2.Fun.Captcha;
using CultOfJakito.UltraTelephone2.Util;
using HarmonyLib;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Chaos.Effects
{
    [RegisterChaosEffect]
    [HarmonyPatch]
    public class ExaggeratedViewmodelLag : ChaosEffect
    {
        [Configgable("Chaos/Effects/Exaggerated Viewmodel Lag", "Exaggerated Viewmodel Lag")]
        private static ConfigToggle s_enabled = new ConfigToggle(true);

        private UniRandom s_rng;
        private GameObject hudCam;

        public override void BeginEffect(UniRandom random)
        {
            s_rng = random;
        }

        private bool done;

        //I want this to run once on the first frame, since the player script needs some time to initialize.
        private void Update()
        {
            if (done)
                return;

            ConvertHudCam();
            done = true;
        }

        private void ConvertHudCam()
        {
            hudCam = NewMovement.Instance.hudCam;

            //Set the hudcam ref to the dummy object so the player script wont change its position.
            GameObject dummyObject = new GameObject("VMDummy");
            dummyObject.transform.position = hudCam.transform.position;
            dummyObject.transform.rotation = hudCam.transform.rotation;
            dummyObject.transform.SetParent(hudCam.transform);

            //Get tricked, dummy.
            NewMovement.Instance.hudCam = dummyObject;

            //add our script to the real cam.
            hudCam.AddComponent<ViewModelSwayer>();
        }

        public override bool CanBeginEffect(ChaosSessionContext ctx) => s_enabled.Value && base.CanBeginEffect(ctx);

        public override int GetEffectCost()
        {
            return 1;
        }

   
        protected override void OnDestroy() {}
    }

    public class ViewModelSwayer : MonoBehaviour
    {
        private Rigidbody watchedBody;
        private Transform cameraReference;
        NewMovement player;

        [Configgable("Chaos/Effects/Exaggerated Viewmodel Lag", "Max Viewmodel Distance")]
        private static ConfigInputField<float> s_maxViewmodelDistance = new ConfigInputField<float>(0.2f);

        [Configgable("Chaos/Effects/Exaggerated Viewmodel Lag", "Viewmodel Distance Randomness")]
        private static ConfigInputField<float> s_maxDistanceRandomness = new ConfigInputField<float>(0.15f);

        [Configgable("Chaos/Effects/Exaggerated Viewmodel Lag", "Velocity Divisor")]
        private static ConfigInputField<float> s_velocityDivisor = new ConfigInputField<float>(350f, (v) =>
        {
            //No dividing by zero!!!
            return v > 0f;
        });

        [Configgable("Chaos/Effects/Exaggerated Viewmodel Lag", "Velocity Direction Multiplier")]
        private static ConfigInputField<float> s_velocityDirectionMultiplier = new ConfigInputField<float>(-1f);

        [Configgable("Chaos/Effects/Exaggerated Viewmodel Lag", "Viewmodel speed")]
        private static ConfigInputField<float> s_viewmodelSpeed = new ConfigInputField<float>(15f);

        UniRandom rng;

        private float maxViewmodelDistance = 0.2f;

        private void Awake()
        {
            this.watchedBody = NewMovement.Instance.rb;
            this.cameraReference = CameraController.Instance.transform;
            player = NewMovement.Instance;

            rng = new UniRandom(new SeedBuilder()
                .WithGlobalSeed()
                .WithSceneName());

            maxViewmodelDistance = s_maxViewmodelDistance.Value * (1+(rng.Float() * s_maxDistanceRandomness.Value));
        }

        //does the viewmodel sway but on OUR terms... :3c
        //This is ripped from player update loop.
        private void Update()
        {
            Vector3 offset = Vector3.ClampMagnitude(player.camOriginalPos - this.cameraReference.InverseTransformDirection(this.watchedBody.velocity) / s_velocityDivisor.Value * s_velocityDirectionMultiplier.Value, maxViewmodelDistance);
            Vector3 localPosition = this.transform.localPosition;
            float dist = Vector3.Distance(offset, localPosition);
            transform.localPosition = Vector3.MoveTowards(localPosition, offset, Time.deltaTime * s_viewmodelSpeed.Value * dist);
        }
    }

}
