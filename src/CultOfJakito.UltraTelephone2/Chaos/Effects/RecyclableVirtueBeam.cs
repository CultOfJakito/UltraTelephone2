using Configgy;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using HarmonyLib;

namespace CultOfJakito.UltraTelephone2.Chaos.Effects;

[RegisterChaosEffect]
[HarmonyPatch]
public class RecyclableVirtueBeam : ChaosEffect
{
    [Configgable("Chaos/Effects", "Better Virtue Beam")]
    private static ConfigToggle s_enabled = new(true);

    private static bool s_effectActive;

    private static float s_timeSinceLastTick;

    public override void BeginEffect(UniRandom random) => s_effectActive = true;
    public override bool CanBeginEffect(ChaosSessionContext ctx) => s_enabled.Value && base.CanBeginEffect(ctx);
    public override int GetEffectCost() => 3;

    [HarmonyPatch(typeof(VirtueInsignia), nameof(VirtueInsignia.Update)), HarmonyPostfix]
    public static void BeamExtension(VirtueInsignia __instance)
    {
        if (!s_effectActive && !s_enabled.Value)
            return;


        __instance.explosionLength = 20f;
        __instance.hasHitTarget = false;
    }

    protected override void OnDestroy() => s_effectActive = false;
}
