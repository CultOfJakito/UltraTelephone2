using Configgy;
using CultOfJakito.UltraTelephone2.Assets;
using CultOfJakito.UltraTelephone2.Chaos;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.zelzmiy;

[RegisterChaosEffect]
internal class HRTBurger : ChaosEffect
{
    private GameObject _estrogenBurger;
    private GameObject _testosteroneBurger;

    [Configgable("Chaos/Effects", "HRT Burgers")]
    private static ConfigToggle s_enabled = new(true);

    private static bool s_effectActive;

    public override void BeginEffect(UniRandom random)
    {
        _estrogenBurger = UT2Assets.ZelzmiyBundle.LoadAsset<GameObject>("estrogen burger.prefab");
        _testosteroneBurger = UT2Assets.ZelzmiyBundle.LoadAsset<GameObject>("testosterone burger.prefab");
        s_effectActive = true;
    }

    public override int GetEffectCost() => 1;

    public override bool CanBeginEffect(ChaosSessionContext ctx) => s_enabled.Value && base.CanBeginEffect(ctx);

    [HarmonyPatch(typeof(ItemIdentifier), "Start")]
    [HarmonyPostfix]
    public void ReplaceSkull(ItemIdentifier __instance, ItemType itemType)
    {
        if (!s_enabled.Value || !s_effectActive)
            return;

        Renderer renderer = __instance.gameObject.GetComponent<Renderer>();

        if (renderer == null)
            return;

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

    public override void Dispose()
    {
        s_effectActive = false;
        base.Dispose();
    }
}
