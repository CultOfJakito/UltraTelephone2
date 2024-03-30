using Configgy;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using HarmonyLib;
using Random = UnityEngine.Random;

namespace CultOfJakito.UltraTelephone2.Chaos.Effects;

[RegisterChaosEffect]
[HarmonyPatch]
public class RandomDashVelocity : ChaosEffect
{
    [Configgable("Chaos/Effects", "Random Velocity")]
    private static ConfigToggle s_enabled = new(true);

    private static bool s_currentlyActive;

    public override void BeginEffect(UniRandom random) => s_currentlyActive = true;
    public override bool CanBeginEffect(ChaosSessionContext ctx) => s_enabled.Value && base.CanBeginEffect(ctx);
    public override int GetEffectCost() => 1;

    [HarmonyPatch(typeof(NewMovement), nameof(NewMovement.Dodge)), HarmonyPostfix]
    public static void SpeedLimit(NewMovement __instance)
    {
        if (!s_currentlyActive && !s_enabled.Value)
            return;
        __instance.rb.velocity *= Random.Range(0.5f, 5f);
    }
}
