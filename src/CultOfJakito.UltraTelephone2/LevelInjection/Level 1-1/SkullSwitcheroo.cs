using Configgy;
using CultOfJakito.UltraTelephone2.Util;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.LevelInjection.Level_1_1
{
    [RegisterLevelInjector]
    public class SkullSwitcheroo : MonoBehaviour, ILevelInjector
    {
        [Configgable("Patches/Level", "Skull Switcheroo")]
        private static ConfigToggle s_enabled = new(true);

        public void OnLevelLoaded(string sceneName)
        {
            if(sceneName != "Level 1-1" || !s_enabled.Value)
            {
                UnityEngine.Object.Destroy(this);
                return;
            }
        }

        const string redOriginAltarPath = "3 - Skull Field/Altar/Cube";
        const string optionalRoomPath = "4C - Optional Room";

        const string blueOriginAltarPath = "11 - Blue Skull Room/11 Nonstuff/Altar/Cube";
        const string longCorridorPath = "10 - Long Corridor/10 Nonstuff";

        private UniRandom random;

        private void Start()
        {
            random = new UniRandom(new SeedBuilder().WithGlobalSeed().WithSceneName());

            if(random.Bool())
            {
                MoveBlueAltar();
                MoveRedAltar();
            }
            else
            {
                if(random.Bool())
                {
                    MoveRedAltar();
                }
                else
                {
                    MoveBlueAltar();
                }
            }
        }

        private void MoveBlueAltar()
        {
            Transform blueAltar = Locator.LocateComponentsOfType<ItemPlaceZone>()
                .FirstOrDefault(x => x.name == "Cube"
            && x.transform.HasParentalPath(blueOriginAltarPath)
            && x.transform.TryGetComponent<ItemPlaceZone>(out ItemPlaceZone ipz)
            && ipz.acceptedItemType == ItemType.SkullBlue).transform.parent;

            if (blueAltar == null)
            {
                Debug.LogError("Red Altar not found.");
                return;
            }

            Transform secretCubbyParent = Locator.LocateComponent<Transform>(longCorridorPath);

            if (secretCubbyParent == null)
            {
                Debug.LogError("Cubby Room not found.");
                return;
            }

            //move the red altar to the optional room :3
            blueAltar.SetParent(secretCubbyParent, true);
            blueAltar.localPosition = new Vector3(-16.532f, 2.619999f, -59.85059f);
            blueAltar.localRotation = Quaternion.Euler(0, 0, 0);
            blueAltar.localScale = new Vector3(0.9000005f, 0.8f, 0.8f);
        }

        private void MoveRedAltar()
        {
            Transform redAltar = Locator.LocateComponentsOfType<ItemPlaceZone>()
                .FirstOrDefault(x => x.name == "Cube"
            && x.transform.HasParentalPath(redOriginAltarPath)
            && x.transform.TryGetComponent<ItemPlaceZone>(out ItemPlaceZone ipz)
            && ipz.acceptedItemType == ItemType.SkullRed).transform.parent;

            if (redAltar == null)
            {
                Debug.LogError("Red Altar not found.");
                return;
            }

            Transform optionalRoom = Locator.LocateComponent<Transform>(optionalRoomPath);

            if (optionalRoom == null)
            {
                Debug.LogError("Optional Room not found.");
                return;
            }

            //move the red altar to the optional room :3
            redAltar.SetParent(optionalRoom, true);
            redAltar.localPosition = new Vector3(-24.33f, 0.07f, -0.08f);
            redAltar.localRotation = Quaternion.Euler(0, 0, 0);
            redAltar.localScale = new Vector3(0.9000005f, 0.8f, 0.8f);
        }
    }
}
