using Configgy;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using HarmonyLib;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Chaos.Effects;

[RegisterChaosEffect]
[HarmonyPatch]
public class KnuckleBlasterRecoil : ChaosEffect
{
    [Configgable("Chaos/Effects", "Knuckle Blaster Recoil")]
    private static ConfigToggle s_enabled = new(true);

    private static bool s_effectActive;

    private static float s_timeSinceLastTick;

    public override void BeginEffect(UniRandom random) => s_effectActive = true;
    public override bool CanBeginEffect(ChaosSessionContext ctx) => s_enabled.Value && base.CanBeginEffect(ctx);
    public override int GetEffectCost() => 1;

    [HarmonyPatch(typeof(Punch), nameof(Punch.BlastCheck)), HarmonyPostfix]
    public static void BeamExtension()
    {
        if (!s_effectActive || !s_enabled.Value)
            return;

        Vector3 currentVelocity = NewMovement.Instance.rb.velocity;
        currentVelocity.x *= -1;
        currentVelocity.y += 50;
        currentVelocity.z *= -1;

        NewMovement.Instance.rb.velocity = currentVelocity;
    }

    protected override void OnDestroy() => s_effectActive = false;
}
