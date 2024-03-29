using Configgy;
using CultOfJakito.UltraTelephone2.Chaos;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using HarmonyLib;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Effects.Jumpscare;

[RegisterChaosEffect]
[HarmonyPatch]
public class JumpscareEffect : ChaosEffect
{
    [Configgable("Chaos/Effects", "Jumpscares")]
    private static ConfigToggle s_enabled = new(true);

    private bool s_effectActive = false;
    private UniRandom _random;

    public override void BeginEffect(UniRandom random)
    {
        _random = random;
        s_effectActive = true;
    }

    public override int GetEffectCost() => 5;
    public override void Dispose()
    {
        s_effectActive = false;
        base.Dispose();
    }

    public override bool CanBeginEffect(ChaosSessionContext ctx) => s_enabled.Value && base.CanBeginEffect(ctx);

    private void Update()
    {
        if (!s_effectActive)
            return;

        if (Input.GetMouseButtonDown(0) && _random.Chance(0.15f))
        {
            UltraTelephone.Hydra.Jumpscare.Scare(true);
        }
    }
}
