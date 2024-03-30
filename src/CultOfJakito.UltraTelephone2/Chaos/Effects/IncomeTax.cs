using Configgy;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using HarmonyLib;
using UnityEngine;

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

        float tax = points * 0.3f;

        //Round up >:3c
        int taxAmount = Mathf.CeilToInt(tax);

        s_taxAmountBuffer = taxAmount;
        points = Mathf.Max(0, points - taxAmount);
    }

    private static int s_taxAmountBuffer;

    [HarmonyPatch(typeof(FinalRank), nameof(FinalRank.PointsShow)), HarmonyPostfix]
    public static void TaxDisplay(FinalRank __instance)
    {
        if (!s_currentlyActive && !s_enabled.Value)
            return;

        if (s_taxAmountBuffer <= 0)
            return;

        __instance.pointsText.text += $"<color=red>(Income Tax 30%) -{s_taxAmountBuffer}</color>";
    }

    protected override void OnDestroy() => s_currentlyActive = false;
}
