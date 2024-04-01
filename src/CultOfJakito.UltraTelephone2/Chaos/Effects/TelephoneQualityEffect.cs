using Configgy;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using CultOfJakito.UltraTelephone2.Events;
using HarmonyLib;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Chaos.Effects;

[RegisterChaosEffect]
public class TelephoneQuality : ChaosEffect
{
    [Configgable("Chaos/Effects", "Telephone Quality")]
    private static ConfigToggle s_enabled = new(true);
    private static bool s_effectActive;
    private AudioLowPassFilter _lp;

    public override void BeginEffect(UniRandom random)
    {
        s_effectActive = true;
        (_lp = CameraController.Instance.gameObject.GetComponent<AudioLowPassFilter>()).cutoffFrequency = 3400;
        CameraController.Instance.gameObject.AddComponent<AudioHighPassFilter>().cutoffFrequency = 300;
    }

    private void Update()
    {
        if (!s_effectActive || !(bool)_lp)
        {
            return;
        }

        _lp.enabled = true; //i cba to do this better fuck you
    }

    protected override void OnDestroy()
    {
        Destroy(_lp);
        Destroy(CameraController.Instance.gameObject.GetComponent<AudioHighPassFilter>());
        s_effectActive = false;
    }

    public override bool CanBeginEffect(ChaosSessionContext ctx) => s_enabled.Value && base.CanBeginEffect(ctx);
    public override int GetEffectCost() => 3;
}
