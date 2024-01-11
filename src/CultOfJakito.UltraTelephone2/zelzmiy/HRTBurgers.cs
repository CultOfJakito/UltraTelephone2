using CultOfJakito.UltraTelephone2.Chaos;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using HarmonyLib;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.zelzmiy
{
    [RegisterChaosEffect]
    internal class HRTBurgers : ChaosEffect
    {

        private GameObject _estrogenBurger;
        private GameObject _testosteroneBurger;

        public override void BeginEffect(System.Random random)
        {
            UltraTelephoneTwo.Instance.BurgerLoader = new("HRT Borgers.resource");

            _estrogenBurger = UltraTelephoneTwo.Instance.BurgerLoader.LoadAsset<GameObject>("estrogen burger");
            _testosteroneBurger = UltraTelephoneTwo.Instance.BurgerLoader.LoadAsset<GameObject>("testosterone burger");

            if (!_estrogenBurger || !_testosteroneBurger)
            {
                Debug.LogError("Burgers Not Loaded!");
            }
        }

        public override int GetEffectCost() => 1;

        [HarmonyPatch(typeof(ItemIdentifier), "Start"), HarmonyPostfix]
        public void ReplaceSkull(ItemIdentifier __instance, ItemType ___itemType)
        {
            Renderer renderer = __instance.gameObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.enabled = false;
                if (___itemType == ItemType.SkullRed)
                {
                    Instantiate(_estrogenBurger, renderer.transform);
                }

                if (___itemType == ItemType.SkullBlue)
                {
                    Instantiate(_testosteroneBurger, renderer.transform);
                }
            }
        }
    }
}
