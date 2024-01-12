using HarmonyLib;
using ULTRAKILL;
using UnityEngine;

[HarmonyPatch]
public class BouncyCannonballPatch
{
    public static PhysicMaterial Bouncy = new PhysicMaterial("Bouncy")
    {
        bounciness = 1f,
        dynamicFriction = 0f,
        staticFriction = 0f,
        frictionCombine = PhysicMaterialCombine.Minimum,
        bounceCombine = PhysicMaterialCombine.Maximum
    };
    [HarmonyPatch(typeof(Cannonball), "Start")]
    [HarmonyPrefix]
    public static void Start(Cannonball __instance)
    {
        BouncyCannonball cb = __instance.gameObject.AddComponent<BouncyCannonball>();
        cb.rb = __instance.GetComponent<Rigidbody>();
    }
    [HarmonyPatch(typeof(Cannonball), "Break")]
    [HarmonyPrefix]
    public static bool Break(Cannonball __instance)
    {
        if(!CannonBallBounce.Enabled.Value || __instance.GetComponent<BouncyCannonball>().RemainingTime <= 0) return true;
        return false;
    }
}
