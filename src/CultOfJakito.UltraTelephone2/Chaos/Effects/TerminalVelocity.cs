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

        if (__instance.rb.velocity.y < -1)
        {
            Vector3 currentVelocity = __instance.rb.velocity;
            currentVelocity.y = -1;
            __instance.rb.velocity = currentVelocity;
        }
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
