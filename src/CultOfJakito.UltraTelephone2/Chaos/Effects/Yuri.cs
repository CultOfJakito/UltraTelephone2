using Configgy;
using CultOfJakito.UltraTelephone2.Assets;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using CultOfJakito.UltraTelephone2.Util;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace CultOfJakito.UltraTelephone2.Chaos.Effects;

/// <summary>
/// Suggestion #298: from: oxblood, "yuri" <br/>
/// Adds an image to the terminal
/// </summary>
[HarmonyPatch]
[RegisterChaosEffect]
internal class Yuri : ChaosEffect
{
    [Configgable("Chaos/Effects", "Terminal Yuri")]
    private static ConfigToggle s_enabled = new(true);

    private static bool s_effectActive;
    private static Sprite s_yuriSprite;

    public override void BeginEffect(UniRandom random)
    {
        s_yuriSprite ??= UT2Assets.GetAsset<Sprite>("Assets/Telephone 2/YURI!!!/Yuri Image.gif");
        s_effectActive = true;
    }

    [HarmonyPatch(typeof(ShopZone), nameof(ShopZone.Start)), HarmonyPostfix]
    public static void AddYuriToShop(ShopZone __instance)
    {
        if (!s_effectActive || !s_enabled.Value)
            return;

        Canvas shopCanvas = __instance.gameObject.GetComponentInChildren<Canvas>(true);

        if (!shopCanvas)
        {
            Debug.Log("Couldn't Find Canvas!");
            return;
        }

        if (__instance.transform.parent != null)
        {
            if (__instance.transform.parent.name == "Barricade")
            {
                //Don't add Yuri to the barricade shops :3
                return;
            }
        }

        Image bg = shopCanvas.transform.LocateComponentInChildren<Image>("Background");
        if (bg == null)
            return;

        bg.sprite = s_yuriSprite;
        bg.color = Color.white;
    }

    public override bool CanBeginEffect(ChaosSessionContext ctx) => s_enabled.Value && base.CanBeginEffect(ctx);

    public override int GetEffectCost() => 1;

    protected override void OnDestroy() => s_effectActive = false;
}
