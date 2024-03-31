using Configgy;
using CultOfJakito.UltraTelephone2.Assets;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using CultOfJakito.UltraTelephone2.Util;
using HarmonyLib;
using UnityEngine;
using Console = GameConsole.Console;

namespace CultOfJakito.UltraTelephone2.Chaos.Effects;

[RegisterChaosEffect]
[HarmonyPatch]
public class BallerInsurrection : ChaosEffect
{
    [Configgable("Chaos/Effects", "Ultraballin")]
    private static ConfigToggle s_enabled = new(true);

    private static bool s_effectActive;
    private static GameObject Ball => UT2Assets.GetAsset<GameObject>("Assets/Telephone 2/Misc/Prefabs/Basketball/Basketball.prefab");

    public override void BeginEffect(UniRandom random)
    {
        s_effectActive = true;
    }

    public override bool CanBeginEffect(ChaosSessionContext ctx) => s_enabled.Value && base.CanBeginEffect(ctx);
    public override int GetEffectCost() => 1;

    [HarmonyPatch(typeof(Sisyphus), nameof(Sisyphus.Start)), HarmonyPostfix]
    public static void BallReplacement(Sisyphus __instance)
    {
        if (!s_effectActive || !s_enabled.Value)
            return;

        //Debug Gore Zone/Sisyphus(Clone)/ArmStretcherRigIKArmOnly/metarig/root/spine/spine.001/spine.002/spine.003/shoulder.L/
        //upper_arm.L/forearm.L/hand.L/hand.L.001/hand.L.002/hand.L.003/hand.L.004/hand.L.005/hand.L.006/
        Transform hand = __instance.transform.GetChild(2).LocateComponentInChildren<Transform>("hand.L.006");
        GameObject ball = Instantiate(Ball, hand);
        ball.transform.localPosition = new Vector3(0, 0, 0);
        ball.transform.localScale = new Vector3(15, 15, 15);

    }

    protected override void OnDestroy() => s_effectActive = false;
}
