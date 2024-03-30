using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;
using Configgy;
using CultOfJakito.UltraTelephone2.Assets;
using CultOfJakito.UltraTelephone2.Util;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.LevelInjection.Level_7_1
{
    [RegisterLevelInjector]
    public class NoFakeMannequins : MonoBehaviour, ILevelInjector
    {
        [Configgable("Patches/Level", "Mannequin Ambush")]
        private static ConfigToggle s_enabled = new ConfigToggle(true);

        public void OnLevelLoaded(string sceneName)
        {
            if(sceneName != "Level 7-1" || !s_enabled.Value)
            {
                UnityEngine.Object.Destroy(this);
                return;
            }
        }

        private void Start()
        {
            ReplaceMannequins(Locator.LocateComponent<Transform>(ambushRoomPath));
            ReplaceMannequins(Locator.LocateComponent<Transform>(ambushRoomPath2));
            ReplaceMannequins(Locator.LocateComponent<Transform>(ambushRoomPath3));
        }

        const string ambushRoomPath = "First Section/Opening Halls Geometry/Opening Nonstuff/Mannequin Room/Dolls (Alerted)/Side 1";
        const string ambushRoomPath2 = "First Section/Opening Halls Geometry/Opening Nonstuff/Mannequin Room/Dolls (Alerted)/Side 1 (1)";
        const string ambushRoomPath3 = "First Section/Opening Halls Geometry/Opening Nonstuff/Walkway Arena";

        const string mannequinPoser = "MannequinPoser";

        private void ReplaceMannequins(Transform parent)
        {
            if (parent == null)
                return;

            foreach (Transform child in parent)
            {
                if (!child.name.Contains(mannequinPoser))
                    continue;

                //These ones are not fake and are real enemies. So dont replace them.
                if (child.GetComponentInParent<GoreZone>() != null)
                    continue;

                if (!child.gameObject.TryGetComponent<ProximityActivatedMannequin>(out ProximityActivatedMannequin man))
                    man = child.gameObject.AddComponent<ProximityActivatedMannequin>();

                man.fake = child.gameObject;
            }
        }
    }

    public class ProximityActivatedMannequin : MonoBehaviour
    {
        public GameObject fake;
        public float radius = 2.5f;
        public Vector3 center = new Vector3(0, 1.25f, 0);

        private SphereCollider col;

        private void Start()
        {
            col = gameObject.AddComponent<SphereCollider>();
            col.isTrigger = true;
            col.radius = radius;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player"))
                return;

            Spawn();
        }

        public void Spawn()
        {
            col.enabled = false;
            fake.SetActive(false);
            Vector3 fakePos = fake.transform.position;
            Quaternion fakeRot = fake.transform.rotation;

            GameObject mannequinPrefab = UkPrefabs.MannequinEnemy.GetObject();
            GameObject mannequinGameObject = GameObject.Instantiate(mannequinPrefab, fakePos, fakeRot);
            mannequinGameObject.SetActive(true);

            GoreZone goreZone = GoreZone.ResolveGoreZone(mannequinGameObject.transform);

            if (goreZone != null)
                mannequinGameObject.transform.SetParent(goreZone.goreZone, true);

            EnemyIdentifier eid = mannequinGameObject.GetComponent<EnemyIdentifier>();
            eid.dontCountAsKills = true;
            eid.spawnIn = false;
        }
    }
}
