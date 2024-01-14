using System;
using System.Collections.Generic;
using System.Text;
using CultOfJakito.UltraTelephone2.Chaos;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2
{
    [RegisterChaosEffect]
    public class GooglyEyesEffect : ChaosEffect
    {
        static bool s_effectActive = false;
        static System.Random random;

        public override void BeginEffect(System.Random rand)
        {
            random = rand;
            s_effectActive = true;
        }


        public override int GetEffectCost()
        {
            return 1;
        }

        private void OnDestroy()
        {
            s_effectActive = false;
        }

    }

    public class GooglyEye : MonoBehaviour
    {
        private Vector2 velocity;
        private Vector2 position;
        private float radius;
        private Vector3 lastPosition;



        private void Awake()
        {
            lastPosition = transform.position;
            velocity = new Vector2(0, 0);
        }

        private void Update()
        {
           
        }

        private void Sol1()
        {
            Vector3 positionalDelta = transform.position - lastPosition;
            Vector3 normal = transform.forward;

            Vector3 localizedVelocity = transform.InverseTransformDirection(positionalDelta);
            localizedVelocity.z = 0;
            velocity += new Vector2(localizedVelocity.x, localizedVelocity.y);

            float gravity = Physics.gravity.y;

            if (position.magnitude >= radius)
            {
                Vector2 tangent = Vector2.Perpendicular(position);
                Vector2 tangentVelo = tangent.normalized * gravity;
            }

            velocity += new Vector2(0, Physics.gravity.y * Time.deltaTime);
        }

        private float energy;
        private float energyDecay;
        private void Sol2()
        {
            energy -= Time.deltaTime * energyDecay;
            //USE SINE WAVE TO MOVE THE EYE
            //SINE OVER TIME DECREASING ENERGY LOSES MOMENTUM
        }

        private void LateUpdate()
        {
            lastPosition = transform.position;
        }

    }
}
