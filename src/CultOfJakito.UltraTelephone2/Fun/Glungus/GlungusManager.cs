using System;
using System.Collections.Generic;
using System.Text;
using Configgy;
using CultOfJakito.UltraTelephone2.Assets;
using CultOfJakito.UltraTelephone2.Util;
using UnityEngine;
using UnityEngine.AI;

namespace CultOfJakito.UltraTelephone2.Fun.Glungus
{
    [ConfigureSingleton(SingletonFlags.NoAutoInstance)]
    public class GlungusManager : MonoSingleton<GlungusManager>
    {
        [Configgable("Chaos/Effects", "Glungus Enabled")]
        public static ConfigToggle S_Enabled = new ConfigToggle(true);

        [Configgable("Fun/Glungus", "Max Glungus")]
        private static ConfigInputField<int> s_maxGlungus = new ConfigInputField<int>(50);

        public Vector3 StandableSpot;
        public Vector3 PlayerPosition;
        public float TickRate = 1f;

        private List<Glungus> glunguses = new List<Glungus>();

        public override void Awake()
        {
            base.Awake();

            S_Enabled.OnValueChanged += SetEnabled;

            if (!S_Enabled.Value)
            {
                gameObject.SetActive(false);
                return;
            }


            SlowTickGlungus();
        }

        private void SetEnabled(bool enabled)
        {
            if (!enabled)
            {
                gameObject.SetActive(false);
                return;
            }

            gameObject.SetActive(true);
        }

        public override void OnEnable()
        {

            SlowTickGlungus();
        }

        public static void SpawnGlungus(Vector3 position)
        {
            if (instance != null)
                instance.SpawnGlungusInternal(position);
        }

        private void SpawnGlungusInternal(Vector3 position)
        {
            glunguses.RemoveAll(x => x == null);
            if (glunguses.Count >= s_maxGlungus.Value)
                return;

            //Check for navmesh position
            PlayerPosition = NewMovement.Instance.transform.position;
            if (NavMesh.SamplePosition(PlayerPosition, out NavMeshHit standableSpot, 3f, -1))
            {
                StandableSpot = standableSpot.position;
            }

            //Spawn glungus
            GameObject glungObject = GameObject.Instantiate(HydraAssets.Glungus, position, Quaternion.identity);
            Glungus glungus = glungObject.AddComponent<Glungus>();
            glunguses.Add(glungus);
            glungus.agent.Warp(StandableSpot);
        }

        private void SlowTickGlungus()
        {
            PlayerPosition = NewMovement.Instance.transform.position;
            if (NavMesh.SamplePosition(PlayerPosition, out NavMeshHit standableSpot, 3f, -1))
            {
                StandableSpot = standableSpot.position;
            }

            //purge null glunguses
            glunguses.RemoveAll(x => x == null);

            for (int i = 0; i < glunguses.Count; i++)
            {
                glunguses[i].UpdateGlungus(this);
            }

            this.DoAfterTimeUnscaled(TickRate, SlowTickGlungus);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            S_Enabled.OnValueChanged -= SetEnabled;
        }
    }
}
