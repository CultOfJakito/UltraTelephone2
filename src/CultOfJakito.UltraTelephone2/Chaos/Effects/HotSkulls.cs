using Configgy;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using HarmonyLib;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Chaos.Effects;

[RegisterChaosEffect]
[HarmonyPatch]
public class HotSkulls : ChaosEffect
{
    [Configgable("Chaos/Effects", "Hot Skulls")]
    private static ConfigToggle s_enabled = new(true);

    private static bool s_effectActive;

    private static float s_timeSinceLastTick;

    public override void BeginEffect(UniRandom random) => s_effectActive = true;
    public override bool CanBeginEffect(ChaosSessionContext ctx) => s_enabled.Value && base.CanBeginEffect(ctx);
    public override int GetEffectCost() => 1;

    [HarmonyPatch(typeof(NewMovement), nameof(NewMovement.Update)), HarmonyPostfix]
    public static void ItBurns(NewMovement __instance)
    {
        if (!s_effectActive || !s_enabled.Value)
            return;

        s_timeSinceLastTick += Time.deltaTime;

        FistControl fist = __instance.GetComponentInChildren<FistControl>();
        ItemIdentifier item = fist.heldObject;

        if (item == null)
            return;

        if (s_timeSinceLastTick > 1 && item.name.Contains("Skull"))
        {
            __instance.GetHurt(3, false);
            s_timeSinceLastTick = 0;
        }
    }

    protected override void OnDestroy() => s_effectActive = false;
}
