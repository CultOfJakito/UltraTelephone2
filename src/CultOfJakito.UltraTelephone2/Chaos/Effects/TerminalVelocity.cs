using Configgy;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using HarmonyLib;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Chaos.Effects;

[RegisterChaosEffect]
[HarmonyPatch]
public class TerminalVelocity : ChaosEffect
{
    [Configgable("Chaos/Effects", "Terminal Velocity")]
    private static ConfigToggle s_enabled = new(true);

    private static bool s_currentlyActive;

    public override void BeginEffect(UniRandom random) => s_currentlyActive = true;
    public override bool CanBeginEffect(ChaosSessionContext ctx) => s_enabled.Value && base.CanBeginEffect(ctx);
    public override int GetEffectCost() => 1;

    [HarmonyPatch(typeof(NewMovement), nameof(NewMovement.FixedUpdate)), HarmonyPostfix]
    public static void SpeedLimit(NewMovement __instance)
    {
        if (!s_currentlyActive && !s_enabled.Value)
            return;

        if (__instance.rb.velocity.y < -1)
        {
            Vector3 currentVelocity = __instance.rb.velocity;
            currentVelocity.y = -1;
            __instance.rb.velocity = currentVelocity;
        }
    }

    protected override void OnDestroy() => s_currentlyActive = false;
}
