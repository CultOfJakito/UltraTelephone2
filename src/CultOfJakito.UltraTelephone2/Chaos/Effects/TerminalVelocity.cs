using Configgy;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using CultOfJakito.UltraTelephone2.Events;
using HarmonyLib;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Chaos.Effects;

[RegisterChaosEffect]
[HarmonyPatch]
public class TerminalVelocity : ChaosEffect
{
    [Configgable("Chaos/Effects", "Terminal Velocity")]
    private static ConfigToggle s_enabled = new(true);

    [Configgable("Chaos/Effects/Terminal Velocity/Terminal Velocity", "Maximum velocity")]
    private static float s_terminalVelocity = 15f;

    private static bool s_effectActive;

    public override void BeginEffect(UniRandom random)
    {
        //Defer activation until the player lands. Otherwise you have to wait like literally 2 mins to land.
        GameEvents.OnPlayerActivated += EnableEffect;
    }

    public override bool CanBeginEffect(ChaosSessionContext ctx) => s_enabled.Value && base.CanBeginEffect(ctx);
    public override int GetEffectCost() => 3;

    [HarmonyPatch(typeof(NewMovement), nameof(NewMovement.FixedUpdate)), HarmonyPostfix]
    public static void SpeedLimit(NewMovement __instance)
    {
        if (!s_effectActive || !s_enabled.Value)
            return;

        __instance.rb.velocity = Vector3.ClampMagnitude(__instance.rb.velocity, s_terminalVelocity);
    }

    private void EnableEffect(PlayerActivatedEvent _)
    {
        s_effectActive = true;
    }

    protected override void OnDestroy()
    {
        s_effectActive = false;
        GameEvents.OnPlayerActivated -= EnableEffect;
    }
}
