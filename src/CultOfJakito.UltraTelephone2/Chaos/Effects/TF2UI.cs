using Configgy;
using CultOfJakito.UltraTelephone2.Assets;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace CultOfJakito.UltraTelephone2.Chaos.Effects;

[RegisterChaosEffect]
[HarmonyPatch]
public class TF2UI : ChaosEffect
{
    [Configgable("Chaos/Effects", "TF2UI")]
    public static ConfigToggle Enabled = new(true);

    private static bool s_currentlyActive;

    private static Sprite s_sprite;

    public override void BeginEffect(UniRandom random)
    {
        s_currentlyActive = true;
        s_sprite = UT2Assets.GetAsset<Sprite>("Assets/Telephone 2/Textures/v1viewmodel.png");
    }

    public override bool CanBeginEffect(ChaosSessionContext ctx) => Enabled.Value && base.CanBeginEffect(ctx);
    public override int GetEffectCost() => 1;

    [HarmonyPrefix, HarmonyPatch(typeof(WeaponHUD), nameof(WeaponHUD.UpdateImage))]
    public static void Patch(ref Sprite icon)
    {
        if (!s_currentlyActive)
        {
            return;
        }
        icon = s_sprite;
    }
}
