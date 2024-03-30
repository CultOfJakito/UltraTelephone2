using Configgy;
using CultOfJakito.UltraTelephone2.Assets;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Fun.Herobrine
{
    public static class HerobrineManager
    {
        private static Herobrine herobrine;
        private static GameObject herobrinePrefab => UT2Assets.GetAsset<GameObject>("Assets/Telephone 2/Herobrine/Herobrine.prefab");

        [Configgable("Fun/Herobrine", "Herobrine")]
        private static ConfigToggle herobrineEnabled = new ConfigToggle(true);

        private static bool initialized;

        public static void Init()
        {
            if (initialized)
                return;

            initialized = true;
            herobrineEnabled.OnValueChanged += SetHerobrineEnabled;
            SetHerobrineEnabled(herobrineEnabled.Value);
        }

        private static void SetHerobrineEnabled(bool enabled)
        {
            if (enabled)
            {
                if (herobrine == null)
                {
                    SpawnHerobrine();
                }
                else
                {
                    herobrine.gameObject.SetActive(true);
                }
            }
            else
            {
                if (herobrine != null)
                {
                    herobrine.gameObject.SetActive(false);
                }
            }
        }

        private static void SpawnHerobrine()
        {
            GameObject herobrineGO = UnityEngine.Object.Instantiate(herobrinePrefab);
            herobrine = herobrineGO.GetComponent<Herobrine>();
            UnityEngine.Object.DontDestroyOnLoad(herobrineGO);
            Debug.Log("SPAWNED HEROBRINE");
        }
    }
}
