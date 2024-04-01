using Configgy;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using HarmonyLib;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CultOfJakito.UltraTelephone2.Chaos.Effects;

[RegisterChaosEffect]
[HarmonyPatch]
public class RandomDashVelocity : ChaosEffect
{
    [Configgable("Chaos/Effects", "Random Velocity")]
    private static ConfigToggle s_enabled = new(true);

    private static bool s_currentlyActive;

    public override void BeginEffect(UniRandom random)
    {
        s_currentlyActive = true;
        NewMovement.Instance.gameObject.AddComponent<KeepPlayerOutOfWobblyZone>();
    }

    public override bool CanBeginEffect(ChaosSessionContext ctx) => s_enabled.Value && base.CanBeginEffect(ctx);
    public override int GetEffectCost() => 3;

    [HarmonyPatch(typeof(NewMovement), nameof(NewMovement.Dodge)), HarmonyPostfix]
    public static void SpeedLimit(NewMovement __instance)
    {
        if (!s_currentlyActive || !s_enabled.Value)
            return;

        __instance.rb.velocity *= Random.Range(0.5f, 5f);
        //This patch wreaks havoc sometimes and shoots the player into the wobbly zone or further, so we need to clamp the velocity to prevent that.
        __instance.rb.velocity = Vector3.ClampMagnitude(__instance.rb.velocity, 1000f); 
    }


    protected override void OnDestroy() => s_currentlyActive = false;
}

public class KeepPlayerOutOfWobblyZone : MonoBehaviour
{
    private void LateUpdate()
    {
        if (transform.position.magnitude > 9000)
        {
            if (StatsManager.Instance.currentCheckPoint != null)
            {
                //teleport the player to the last checkpoint
                transform.position = StatsManager.Instance.currentCheckPoint.transform.position + Vector3.up * 1.25f;
            }
            else
            {
                transform.position = Vector3.zero;
            }
        }
    }
}
