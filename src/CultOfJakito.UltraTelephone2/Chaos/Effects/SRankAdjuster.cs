using Configgy;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using HarmonyLib;


namespace CultOfJakito.UltraTelephone2.Chaos.Effects;

[RegisterChaosEffect]
[HarmonyPatch]
public class SRankAdjuster : ChaosEffect
{
    [Configgable("Chaos/Effects", "SRankHelper")]
    private static ConfigToggle s_enabled = new(true);

    private static bool s_currentlyActive;

    private static float s_timeSinceLastTick;

    public override void BeginEffect(UniRandom random) => s_currentlyActive = true;
    public override bool CanBeginEffect(ChaosSessionContext ctx) => s_enabled.Value && base.CanBeginEffect(ctx);
    public override int GetEffectCost() => 1;

    [HarmonyPatch(typeof(StatsManager), nameof(StatsManager.SendInfo)), HarmonyPrefix]
    public static void RankDecreaser(StatsManager __instance)
    {
        if (!s_currentlyActive && !s_enabled.Value)
            return;

        __instance.rankScore -= 1;
        __instance.timeRanks[3] = (int)__instance.seconds - 10;
    }

    protected override void OnDestroy() => s_currentlyActive = false;
}
