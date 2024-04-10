using CultOfJakito.UltraTelephone2.Assets;
using CultOfJakito.UltraTelephone2.Util;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.LevelInjection.Museum
{
    [RegisterLevelInjector]
    internal class CreditsMuseum : ILevelInjector
    {

        public GameObject _creditsEntrance = null;
        public void OnLevelLoaded(string sceneName)
        {
            if (sceneName.Equals("CreditsMuseum2"))
            {
                _creditsEntrance = UT2Assets.GetAsset<GameObject>("Assets/Telephone 2/Credits/COJCreditsRoomEntrence.prefab");
                PlaceCreditEntrance();
            }
        }

        public void PlaceCreditEntrance()
        {
            GameObject floorWater = Locator.LocateComponentsOfType<Transform>().FirstOrDefault(x => x.name == "Floor Water").gameObject;
            GameObject.Instantiate(_creditsEntrance, floorWater.transform.position, Quaternion.identity);
            floorWater.SetActive(false);

        }
    }
}
