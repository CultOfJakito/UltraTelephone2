using Configgy;
using CultOfJakito.UltraTelephone2.Assets;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using HarmonyLib;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Chaos.Effects;

/// <summary>
/// suggestion #314: from:bobot, "add consumable estrogen burger" <br/>
/// Replaces the Blue and red skulls with burgers that have 'T' and "E' written on them respectively
/// </summary>
[RegisterChaosEffect]
[HarmonyPatch]
internal class HRTBurger : ChaosEffect
{
    private static GameObject _estrogenBurger;
    private static GameObject _testosteroneBurger;

    [Configgable("Chaos/Effects", "HRT Burgers")]
    private static ConfigToggle s_enabled = new(true);

    private static bool s_effectActive;

    public override void BeginEffect(UniRandom random)
    {
        _estrogenBurger ??= UT2Assets.GetAsset<GameObject>("Assets/Telephone 2/HRT Borgers/estrogen burger.prefab");
        _testosteroneBurger ??= UT2Assets.GetAsset<GameObject>("Assets/Telephone 2/HRT Borgers/testosterone burger.prefab");
        s_effectActive = true;
    }

    public override int GetEffectCost() => 1;

    public override bool CanBeginEffect(ChaosSessionContext ctx) => s_enabled.Value && base.CanBeginEffect(ctx);

    [HarmonyPatch(typeof(Skull), nameof(Skull.Start))]
    [HarmonyPostfix]
    public static void ReplaceSkull(Skull __instance)
    {
        if (!s_enabled.Value || !s_effectActive)
            return;

        Renderer renderer = __instance.gameObject.GetComponent<Renderer>();

        if (renderer == null)
            return;

        renderer.enabled = false;
        ItemType itemType = __instance.gameObject.GetComponent<ItemIdentifier>().itemType;
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

    protected override void OnDestroy() => s_effectActive = false;
}
