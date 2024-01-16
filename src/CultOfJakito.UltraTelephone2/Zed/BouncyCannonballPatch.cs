using HarmonyLib;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Zed;

[HarmonyPatch]
public class BouncyCannonballPatch
{
    public static PhysicMaterial Bouncy = new("Bouncy")
    {
        bounciness = 1f,
        dynamicFriction = 0f,
        staticFriction = 0f,
        frictionCombine = PhysicMaterialCombine.Minimum,
        bounceCombine = PhysicMaterialCombine.Maximum
    };

    [HarmonyPatch(typeof(Cannonball), "Start")]
    [HarmonyPrefix]
    public static void Start(Cannonball instance)
    {
        BouncyCannonball cb = instance.gameObject.AddComponent<BouncyCannonball>();
        cb.Rb = instance.GetComponent<Rigidbody>();
    }

    [HarmonyPatch(typeof(Cannonball), "Break")]
    [HarmonyPrefix]
    public static bool Break(Cannonball instance) => !CannonBallBounce.Enabled.Value || instance.GetComponent<BouncyCannonball>().RemainingTime <= 0;
}
