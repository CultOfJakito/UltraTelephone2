using Configgy;
using CultOfJakito.UltraTelephone2.Assets;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using HarmonyLib;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Chaos.Effects;

[RegisterChaosEffect]
[HarmonyPatch]
public class SkibidiToilet : ChaosEffect
{
    [Configgable("Chaos/Effects", "Skibidi Toilet")]
    private static ConfigToggle s_enabled = new(true);
    private static bool s_effectActive;
    private static float s_timeSinceLastTick;
    private static UniRandom s_rand;

    public override void BeginEffect(UniRandom random)
    {
        s_effectActive = true;
        s_rand = random;
    }
    public override bool CanBeginEffect(ChaosSessionContext ctx) => s_enabled.Value && base.CanBeginEffect(ctx);
    public override int GetEffectCost() => 3;

    [HarmonyPatch(typeof(Zombie), nameof(Zombie.Awake)), HarmonyPostfix]
    public static void BeamExtension(Zombie __instance)
    {
        if (!s_effectActive && !s_enabled.Value)
            return;

        if (!s_rand.Chance(0.2f))
        {
            return;
        }

        __instance.GetComponent<EnemyIdentifier>().Death();
        Instantiate(UT2Assets.GetAsset<GameObject>("Assets/Telephone 2/Skibidi Toilet/Enemies/Skibidi Toilet.prefab"), __instance.transform.position, __instance.transform.rotation);
    }

    protected override void OnDestroy() => s_effectActive = false;
}
