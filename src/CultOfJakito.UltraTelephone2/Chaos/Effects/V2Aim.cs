using Configgy;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using HarmonyLib;

namespace CultOfJakito.UltraTelephone2.Chaos.Effects;

[RegisterChaosEffect]
[HarmonyPatch]
public class V2Aim : ChaosEffect
{
    [Configgable("Chaos/Effects", "V2 God Aim")]
    private static ConfigToggle s_enabled = new(true);

    private static bool s_currentlyActive;

    public override void BeginEffect(UniRandom random) => s_currentlyActive = true;
    public override bool CanBeginEffect(ChaosSessionContext ctx) => s_enabled.Value && base.CanBeginEffect(ctx);
    public override int GetEffectCost() => 1;

    [HarmonyPatch(typeof(V2), nameof(V2.Update)), HarmonyPostfix]
    public static void SpeedLimit(V2 __instance)
    {
        if (!s_currentlyActive && !s_enabled.Value)
            return;

        __instance.predictAmount = 0.2f;
    }

    protected override void OnDestroy() => s_currentlyActive = false;
}
