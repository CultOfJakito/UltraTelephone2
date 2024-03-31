using Configgy;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using HarmonyLib;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Chaos.Effects;

[RegisterChaosEffect]
[HarmonyPatch]
public class InvertMomentumCoins : ChaosEffect
{
    [Configgable("Chaos/Effects", "Inverted Coin Momentum")]
    private static ConfigToggle s_enabled = new(true);

    private static bool s_currentlyActive;

    public override void BeginEffect(UniRandom random) => s_currentlyActive = true;
    public override bool CanBeginEffect(ChaosSessionContext ctx) => s_enabled.Value && base.CanBeginEffect(ctx);
    public override int GetEffectCost() => 4;

    [HarmonyPostfix, HarmonyPatch(typeof(Coin), nameof(Coin.Start))]
    public static void Patch(Coin __instance)
    {
        if (!s_currentlyActive || !s_enabled.Value)
            return;

        __instance.gameObject.AddComponent<CoinMomentumInverter>();
    }

    protected override void OnDestroy() => s_currentlyActive = false;

}

public class CoinMomentumInverter : MonoBehaviour
{
    private Rigidbody rb;
    private Vector3 initVelocity;
    private bool coinInit = false;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        initVelocity = rb.velocity;
    }

    private void Update()
    {
        if (coinInit)
        {
            return;
        }
        if (rb.velocity == initVelocity)
        {
            return;
        }
        Vector3 velocity = rb.velocity;
        velocity.x *= -1;
        velocity.z *= -1;
        rb.velocity = velocity;

        coinInit = true;
    }
}
