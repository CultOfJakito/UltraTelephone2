using Configgy;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using HarmonyLib;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Chaos.Effects;

[RegisterChaosEffect]
[HarmonyPatch]
public class RecoilEffect : ChaosEffect
{
    [Configgable("Chaos/Effects", "Recoil")]
    private static ConfigToggle s_enabled = new(true);

    private static bool s_effectActive;

    public override void BeginEffect(UniRandom random)
    {
        s_effectActive = true;
    }

    public override bool CanBeginEffect(ChaosSessionContext ctx) => s_enabled.Value && base.CanBeginEffect(ctx);
    public override int GetEffectCost() => 3;
    protected override void OnDestroy() => s_effectActive = false;
    private static float FlooredMultiplier => NewMovement.Instance.gc.touchingGround ? 2 : 1;
    private static float _inertia;

    private static void AddRecoil(float strength)
    {
        if (!s_effectActive)
        {
            return;
        }

        NewMovement.Instance.rb.AddForce(strength * -CameraController.Instance.transform.forward * FlooredMultiplier, ForceMode.VelocityChange);
        _inertia += strength;
    }

    private void Update()
    {
        CameraController.Instance.rotationX += _inertia * Time.deltaTime * 10f;
        _inertia = Mathf.MoveTowards(_inertia, 0, Time.deltaTime * 50);
    }

    [HarmonyPatch(typeof(Revolver), nameof(Revolver.Shoot)), HarmonyPrefix]
    private static void AddRevolverRecoil(Revolver __instance, int shotType)
    {
        if (shotType == 2)
        {
            float multiplier = __instance.pierceShotCharge / 100;
            AddRecoil(20 * multiplier);
        }
        else
        {
            AddRecoil(10);
        }
    }

    [HarmonyPatch(typeof(Shotgun), nameof(Shotgun.Shoot)), HarmonyPrefix]
    private static void AddShotgunRecoil(Shotgun __instance)
    {
        float multiplier = 1 + (__instance.variation == 1 ? (__instance.primaryCharge + 1) / 3 : 0);
        AddRecoil(15 * multiplier);
    }

    [HarmonyPatch(typeof(Shotgun), nameof(Shotgun.ShootSinks)), HarmonyPrefix]
    private static void AddShotgunCoreRecoil(Shotgun __instance)
    {
        AddRecoil(10 * (__instance.grenadeForce / 30 * 2));
    }

    [HarmonyPatch(typeof(Nailgun), nameof(Nailgun.Shoot)), HarmonyPrefix]
    private static void AddNailRecoil()
    {
        AddRecoil(0.75f);
    }

    [HarmonyPatch(typeof(Nailgun), nameof(Nailgun.ShootMagnet)), HarmonyPrefix]
    private static void AddNailMagnetRecoil()
    {
        AddRecoil(10);
    }

    [HarmonyPatch(typeof(Railcannon), nameof(Railcannon.Shoot)), HarmonyPrefix]
    private static void AddRailRecoil()
    {
        AddRecoil(50);
    }

    [HarmonyPatch(typeof(RocketLauncher), nameof(RocketLauncher.Shoot)), HarmonyPrefix]
    private static void AddRocketRecoil()
    {
        AddRecoil(15);
    }


    [HarmonyPatch(typeof(RocketLauncher), nameof(RocketLauncher.ShootCannonball)), HarmonyPrefix]
    private static void AddRocketCannonballRecoil(RocketLauncher __instance)
    {
        AddRecoil(30 * __instance.cbCharge);
    }
}
