using Configgy;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using HarmonyLib;

namespace CultOfJakito.UltraTelephone2.Chaos.Effects;

[RegisterChaosEffect]
[HarmonyPatch]
public class SlowTram : ChaosEffect
{
    [Configgable("Chaos/Effects", "Tram Speed")]
    private static ConfigToggle s_enabled = new(true);

    private static bool s_currentlyActive;

    public override void BeginEffect(UniRandom random) => s_currentlyActive = true;
    public override bool CanBeginEffect(ChaosSessionContext ctx) => s_enabled.Value && base.CanBeginEffect(ctx);
    public override int GetEffectCost() => 0; //free since trams arent used often

    [HarmonyPatch(typeof(TramControl), nameof(TramControl.UpdateSpeedIndicators)), HarmonyPostfix]
    public static void SpeedLimit(TramControl __instance)
    {
        if (!s_currentlyActive && !s_enabled.Value)
            return;

        __instance.speedMultiplier = 10;
        __instance.targetTram.speed = 0;
    }

    protected override void OnDestroy() => s_currentlyActive = false;
}
