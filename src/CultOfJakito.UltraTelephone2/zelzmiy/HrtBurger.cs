using CultOfJakito.UltraTelephone2.Chaos;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using HarmonyLib;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.zelzmiy;

[RegisterChaosEffect]
internal class HrtBurger : ChaosEffect
{
    private GameObject _estrogenBurger;
    private GameObject _testosteroneBurger;

    public override void BeginEffect(UniRandom random)
    {
        _estrogenBurger = UltraTelephoneTwo.Instance.ZelzmiyBundle.LoadAsset<GameObject>("estrogen burger.prefab");
        _testosteroneBurger = UltraTelephoneTwo.Instance.ZelzmiyBundle.LoadAsset<GameObject>("testosterone burger.prefab");

        if (!_estrogenBurger || !_testosteroneBurger)
        {
            Debug.LogError("Burgers Not Loaded!");
        }
    }

    public override int GetEffectCost() => 1;

    [HarmonyPatch(typeof(ItemIdentifier), "Start")]
    [HarmonyPostfix]
    public void ReplaceSkull(ItemIdentifier __instance, ItemType itemType)
    {
        Renderer renderer = __instance.gameObject.GetComponent<Renderer>();

        if (renderer == null)
        {
            return;
        }

        renderer.enabled = false;

        switch (itemType)
        {
            case ItemType.SkullRed:
                Instantiate(_estrogenBurger, renderer.transform);
                break;
            case ItemType.SkullBlue:
                Instantiate(_testosteroneBurger, renderer.transform);
                break;
            default:
                renderer.enabled = true;
                break;
        }
    }
}
