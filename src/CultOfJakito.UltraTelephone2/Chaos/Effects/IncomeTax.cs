using Configgy;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using HarmonyLib;

namespace CultOfJakito.UltraTelephone2.Chaos.Effects;

[RegisterChaosEffect]
[HarmonyPatch]
public class IncomeTax : ChaosEffect
{
    [Configgable("Chaos/Effects", "Taxes")]
    private static ConfigToggle s_enabled = new(true);

    private static bool s_currentlyActive;

    private static float s_timeSinceLastTick;

    public override void BeginEffect(UniRandom random) => s_currentlyActive = true;
    public override bool CanBeginEffect(ChaosSessionContext ctx) => s_enabled.Value && base.CanBeginEffect(ctx);
    public override int GetEffectCost() => 1;

    [HarmonyPatch(typeof(FinalRank), nameof(FinalRank.AddPoints)), HarmonyPrefix]
    public static void Tax(ref int points)
    {
        if (!s_currentlyActive && !s_enabled.Value)
            return;
        float afterTax = points * 0.7f;
        points = (int)afterTax;
    }

    [HarmonyPatch(typeof(FinalRank), nameof(FinalRank.PointsShow)), HarmonyPostfix]
    public static void TaxDisplay(FinalRank __instance)
    {
        if (!s_currentlyActive && !s_enabled.Value)
            return;
        __instance.pointsText.text += "<color=red>(Income Tax -30%)</color>";
    }
}
